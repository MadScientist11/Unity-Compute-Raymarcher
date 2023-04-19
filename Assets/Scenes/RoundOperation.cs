using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundOperation : MonoBehaviour, IOperation
{
    public Operation OperationId { get; set; } = Operation.Round;
    public Vector4 Value => new Vector4(_radius,0);
    [SerializeField] private float _radius;
}
