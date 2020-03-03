﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO.Events;

public class PresenceManager : MonoBehaviour
{
    public SocketIOController io;
    public GameObject RemoteUserPrefab;
    public string SocketId;

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
    }

    #region -- Server Messages --
    private void onConnect(SocketIOEvent e)
    {
        Debug.Log("Socket connected");
        io.Emit("💬join", "\"exampleRoom\"");
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
        remoteUsers[userPoseInfo.SocketId].UpdatePose(userPoseInfo);
    }

    #region -- Public Methods --
    public void TransmitLocalUser(CharacterPoseInfo PoseInfo)
    {
        if (!isReady) return;

        io.Emit("userPose", JsonUtility.ToJson(PoseInfo));
    }
    #endregion

    #region -- Private Methods --
    void addRemoteUser(string socketId)
    {
        //if (socketId == this.SocketId) return; //TODO: reenable this when local mirror testing is complete
        if (remoteUsers.ContainsKey(socketId)) return;

        var remoteUserObject = Instantiate(RemoteUserPrefab);
        var remoteUser = remoteUserObject.GetComponent<RemoteUser>();

        remoteUser.Parent = this;
        remoteUser.SocketId = socketId;
        remoteUser.gameObject.name = "Remote User: " + socketId;
        remoteUser.transform.SetParent(this.transform);
        remoteUser.transform.localPosition = Random.insideUnitSphere;
        remoteUsers.Add(socketId, remoteUser);
    }

    void removeRemoteUser(string socketId)
    {
        if (!remoteUsers.ContainsKey(socketId)) return;

        var remoteUser = remoteUsers[socketId];
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