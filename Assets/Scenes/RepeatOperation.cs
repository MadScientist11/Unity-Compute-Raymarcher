using UnityEngine;

public class RepeatOperation : MonoBehaviour, IOperation
{
    public Operation OperationId { get; set; } = Operation.Repeat;

    public Vector4 Value => new(_value.x, _value.y, _value.z, (float)_valueRepeat);

    [SerializeField] [Min(0)] private Vector3 _value;
    [SerializeField] [Min(0)] private int _valueRepeat;
}