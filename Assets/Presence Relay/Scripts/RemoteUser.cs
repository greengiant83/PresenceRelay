using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteUser : MonoBehaviour
{
    public string SocketId;
    public PresenceManager Parent;

    public Transform Head;
    public Transform LeftController;
    public Transform RightController;

    public void UpdatePose(CharacterPoseInfo UserPoseInfo)
    {
        UserPoseInfo.Head.ToTransform(Head);
        UserPoseInfo.LeftController.ToTransform(LeftController);
        UserPoseInfo.RightController.ToTransform(RightController);
    }
}
