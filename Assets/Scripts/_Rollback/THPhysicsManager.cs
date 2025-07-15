using System.Collections.Generic;
using Mathematics.Fixed;
using NUnit.Framework;

public class THPhysicsManager
{
    List<THPhysicsObject> physicsObjects = new();
    
    public void RegisterPhysicsObject(THPhysicsObject player1)
    {
        if (!physicsObjects.Contains(player1))
        {
            physicsObjects.Add(player1);
        }
    }

    public void ResolveCollisions(FRect positionBounds)
    {
        foreach (var physObject in physicsObjects)
        {
            FVector2 difference = FVector2.Zero;
            if (physObject.GetPositionalBounds().MinX < positionBounds.MinX)
            {
                difference -= new FVector2(physObject.GetPositionalBounds().MinX - (positionBounds.MinX + FP.Epsilon), FP.Zero);
            }
            if (physObject.GetPositionalBounds().MaxX > positionBounds.MaxX)
            {
                difference -= new FVector2(physObject.GetPositionalBounds().MaxX - (positionBounds.MaxX - FP.Epsilon), FP.Zero);
            }
            if (physObject.GetPositionalBounds().MinY < positionBounds.MinY)
            {
                difference -= new FVector2(FP.Zero, physObject.GetPositionalBounds().MinY - (positionBounds.MinY + FP.Epsilon));
            }
            if (physObject.GetPositionalBounds().MaxY > positionBounds.MaxY)
            {
                difference -= new FVector2(FP.Zero, physObject.GetPositionalBounds().MaxY - (positionBounds.MaxY - FP.Epsilon));
            }

            if (difference.X != FP.Zero)
            {
                physObject.ZeroHorizontal();
            }
            physObject.MoveToLocation(physObject.transform.position + difference);
        }
    }
}