
using System;
using Mathematics.Fixed;

public interface IShape
{
    void SetPosition(FVector2 attachedShapeOffset);
    FVector2 GetCenter();
}

[Serializable]
public struct FRect : IShape
{
    public FVector2 topLeft;
    public FVector2 bottomRight;
    public FP height;
    public FP width;
    public FVector2 center;

    public FRect(FVector2 topLeft, FVector2 bottomRight)
    {
        this.topLeft = topLeft;
        this.bottomRight = bottomRight;
        height = FMath.Abs(topLeft.Y - bottomRight.Y);
        width = FMath.Abs(topLeft.X - bottomRight.X);
        center = (topLeft + bottomRight) / 2;
    }

    public FRect(FP height, FP width, FVector2 center)
    {
        this.height = height;
        this.width = width;
        this.center = center;
        topLeft = new();
        bottomRight = new();
        SetPosition(center);
    }

    public FP MinX => topLeft.X;
    public FP MaxX => bottomRight.X;
    public FP MinY => bottomRight.Y;
    public FP MaxY => topLeft.Y;

    public void SetPosition(FVector2 newPosition)
    {
        center = newPosition;
        topLeft = new FVector2(center.X - width / 2, center.Y + height / 2);
        bottomRight = new FVector2(center.X + width / 2, center.Y - height / 2);
    }

    public FVector2 GetCenter()
    {
        return center;
    }
}

[Serializable]
public struct FCircle : IShape
{
    public FVector2 centerPoint;
    public FP radius;

    public FCircle(FVector2 centerPoint, FP radius)
    {
        this.centerPoint = centerPoint;
        this.radius = radius;
    }

    public void SetPosition(FVector2 newPosition)
    {
        centerPoint = newPosition;
    }

    public FVector2 GetCenter()
    {
        return centerPoint;
    }
}