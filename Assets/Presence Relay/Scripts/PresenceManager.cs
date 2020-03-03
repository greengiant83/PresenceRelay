using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnitySocketIO.Events;

[System.Serializable]
public class PeerEvent : UnityEvent<RemoteUser> { }

public class PresenceManager : MonoBehaviour
{
    public PeerEvent OnPeerJoined;
    public PeerEvent OnPeerDropped;
    public SocketIOController io;
    public GameObject RemoteUserPrefab;
    public LocalUser LocalUser;
    public string RoomName = "PresenceRelay";
    public string SocketId;
    public float TransmitRateInSeconds = 0.1f;

    private Dictionary<string, RemoteUser> remoteUsers = new Dictionary<string, RemoteUser>();
    private bool isReady;

    void Start()
    {
        io.On("connect", onConnect);
        io.On("💬joined", onRoomJoined);
        io.On("💬youare", onYouAreMessageReceived);
        io.On("💬listeners", onListenersReceived);
        io.On("💬peerjoin", onPeerJoined);
        io.On("💬peerdrop", onPeerDropped);
        io.On("userPose", onUserPoseReceived);
        io.Connect();

        transmit();
    }

    #region -- Server Messages --
    private void onConnect(SocketIOEvent e)
    {
        Debug.Log("Socket connected");
        io.Emit("💬join", quote(RoomName));
        io.Emit("💬whoami");
    }

    private void onRoomJoined(SocketIOEvent e)
    {
    }

    private void onYouAreMessageReceived(SocketIOEvent e)
    {
        this.SocketId = stripQuotes(e.data);
        io.Emit("💬whoslistening");
    }

    private void onListenersReceived(SocketIOEvent e)
    {
        var reply = JsonUtility.FromJson<ListenersReplyMessage>(e.data);
        if (reply != null)
        {
            foreach (var room in reply.rooms)
            {
                foreach (var socket in room.sockets)
                {
                    addRemoteUser(socket);
                }
            }

            Debug.Log("Socket Ready");
            isReady = true;
        }
        else
        {
            Debug.LogError("Listener Reply was null or couldnt be deserialized");
        }
    }

    private void onPeerJoined(SocketIOEvent e)
    {
        addRemoteUser(stripQuotes(e.data));
    }

    private void onPeerDropped(SocketIOEvent e)
    {
        removeRemoteUser(stripQuotes(e.data));
    }
    #endregion

    private void onUserPoseReceived(SocketIOEvent e)
    {
        var userPoseInfo = JsonUtility.FromJson<CharacterPoseInfo>(e.data);
        if (remoteUsers.ContainsKey(userPoseInfo.SocketId))
        {
            remoteUsers[userPoseInfo.SocketId].UpdatePose(userPoseInfo);
        }
        else
        {
            Debug.Log("Invalid socket id: " + userPoseInfo.SocketId);
        }
    }
    
    #region -- Private Methods --
    void transmit()
    {
        if (isReady)
        {
            LocalUser.UpdatePoseInfo();
            io.Emit("userPose", JsonUtility.ToJson(LocalUser.Pose));
        }

        Invoke("transmit", TransmitRateInSeconds);
    }

    void addRemoteUser(string socketId)
    {
        if (socketId == this.SocketId) return;
        if (remoteUsers.ContainsKey(socketId)) return;

        var remoteUserObject = Instantiate(RemoteUserPrefab);
        var remoteUser = remoteUserObject.GetComponent<RemoteUser>();

        remoteUser.Parent = this;
        remoteUser.SocketId = socketId;
        remoteUser.gameObject.name = "Remote User: " + socketId;
        remoteUser.transform.SetParent(this.transform);
        remoteUser.transform.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
        remoteUser.transform.localRotation = Quaternion.identity;
        remoteUsers.Add(socketId, remoteUser);

        OnPeerJoined.Invoke(remoteUser);
    }

    void removeRemoteUser(string socketId)
    {
        if (!remoteUsers.ContainsKey(socketId)) return;

        var remoteUser = remoteUsers[socketId];
        OnPeerDropped.Invoke(remoteUser);
        Destroy(remoteUser.gameObject);
    }

    private string stripQuotes(string s)
    {
        //Raw strings received over socket are sandwiched in double quotes
        //this function removes them

        if(s.StartsWith("\"") && s.EndsWith("\""))
        {
            s = s.Substring(1, s.Length- 2);
        }
        return s;
    }

    private string quote(string s)
    {
        return "\"" + s + "\"";
    }
    #endregion
}

#region -- Json Data Structures --
// 💬listeners ({ rooms: [{ room: "theRoomKey", sockets: ["123", ...] }, ...] })

[System.Serializable]
public class ListenersReplyMessage
{
    public RoomSocketData[] rooms;
}

[System.Serializable]
public struct RoomSocketData
{
    public string room;
    public string[] sockets;
}
#endregion