using Mathematics.Fixed;
using SharedGame;
using UnityEngine;

public static class THStatics
{
    public static THGameManager GM => (THGameManager)GameManager.Instance;

    public static FP KinematicEquation(FP acceleration, FP velocity, FP position, int time)
    {
        return acceleration * time * time / 2 + velocity * time + position;
    }

    public static Vector3 FVector2ToVector3(FVector2 vector, float z)
    {
        return new Vector3(
            vector.X.ToFloat(),
            vector.Y.ToFloat(),
            z);
    }
}