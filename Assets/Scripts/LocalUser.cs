using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalUser : MonoBehaviour
{
    public PresenceManager Parent;
    public Transform Head;
    public Transform LeftController;
    public Transform RightController;

    private CharacterPoseInfo pose;

    void LateUpdate()
    {
        pose.SocketId = Parent.SocketId;
        pose.Head.FromTransform(Head);
        pose.LeftController.FromTransform(LeftController);
        pose.RightController.FromTransform(RightController);

        Parent.TransmitLocalUser(pose);
    }
}
