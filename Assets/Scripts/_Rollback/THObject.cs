using System;
using System.Collections.Generic;
using Mathematics.Fixed;

public struct FTransform
{
    public FVector2 position;
    public FRotation2D rotation;
    public FVector2 scale;
}

public struct FAttachedShape
{
    public IShape shape;
    public FVector2 offset;

    public FAttachedShape(IShape shape, FVector2 offset)
    {
        this.shape = shape;
        this.offset = offset;
    }
}

public class THObject
{
    public FTransform transform;
    public List<FAttachedShape> attachedShapes = new();

    public THObject(FVector2 position)
    {
        transform = new FTransform(){position = position};
        MoveToLocation(transform.position);
    }

    public void AttachShape(IShape shape)
    {
        FAttachedShape newShape = new(shape, shape.GetCenter() - transform.position);
        attachedShapes.Add(newShape);
    }
    
    public void MoveToLocation(FVector2 targetLocation)
    {
        transform.position = targetLocation;
        UpdateAllAttachedShapes();
        TriggerMoved(transform);
    }

    public Action<FTransform> OnMoved;
    private void TriggerMoved(FTransform newTransform)
    {
        OnMoved?.Invoke(newTransform);
    }

    protected void UpdateAllAttachedShapes()
    {
        foreach (var attachedShape in attachedShapes)
        {
            attachedShape.shape.SetPosition(transform.position + attachedShape.offset);
        }
    }
}