using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct MoveToRandomDestinationJob : IJob
{
    [ReadOnly]
    public NativeArray<Vector2> InputArray;
    [WriteOnly]
    public NativeArray<Vector2> OutputArray;

    public Vector2 Position;
    public float Delta;
    public float Speed;

    public void Execute()
    {
        var newPosition = InputArray[0];
        newPosition = Vector2.Lerp(Position, InputArray[0], Delta);
        OutputArray[0] = newPosition;
    }
}
