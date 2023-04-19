using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionOperation : MonoBehaviour, IOperation
{
    public Operation OperationId { get; set; } = Operation.Onion;

    public Vector4 Value => new Vector4(Radius,0);
    public float Radius;
}