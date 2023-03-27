using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionOperation : MonoBehaviour, IOperation
{
    public int OperationId { get; set; } = 3;
    public Vector4 Value => new Vector4(Radius,0);
    public float Radius;
}
