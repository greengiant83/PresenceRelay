using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PoseInfo
{
    public bool IsVisible;
    public Vector3 Position;
    public Quaternion Rotation;

    public void FromTransform(Transform transform)
    {
        IsVisible = transform.gameObject.activeSelf;
        Position = transform.localPosition;
        Rotation = transform.localRotation;
    }

    public void ToTransform(Transform transform)
    {
        transform.gameObject.SetActive(IsVisible);
        transform.localPosition = Position;
        transform.localRotation = Rotation;
    }
}

[Serializable]
public struct CharacterPoseInfo
{
    public string SocketId;
    public PoseInfo Head;
    public PoseInfo LeftController;
    public PoseInfo RightController;
}
