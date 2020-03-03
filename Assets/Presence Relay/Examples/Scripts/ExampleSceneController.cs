using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSceneController : MonoBehaviour
{
    public void OnPeerJoined(RemoteUser NewUser)
    {
        Debug.Log("Looks like we got a new user: " + NewUser.SocketId);
    }

    public void OnPeerDropped(RemoteUser DroppedUser)
    {
        Debug.Log("This user left: " + DroppedUser.SocketId);
    }
}
