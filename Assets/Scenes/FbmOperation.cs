using System;
using UnityEngine;

public class FbmOperation : MonoBehaviour, IOperation
{
    public Operation OperationId { get; set; } = Operation.FBM;

    public Vector4 Value => new Vector4(Speed,Strength);
    public float Speed = 0.01f;
    public float Strength = 1;
}