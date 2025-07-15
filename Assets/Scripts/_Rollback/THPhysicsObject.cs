using System.Collections.Generic;
using Mathematics.Fixed;

public enum AppliedForceTypes
{
    Impulse,
    Force,
}

public struct THAppliedForce
{
    public int handle;
    public FVector2 appliedForce;
    public AppliedForceTypes appliedForceType;

    public THAppliedForce(int handle, FVector2 appliedForce, AppliedForceTypes appliedForceType)
    {
        this.handle = handle;
        this.appliedForce = appliedForce;
        this.appliedForceType = appliedForceType;
    }

    public void Invalidate()
    {
        handle = -1;
    }

    public bool IsValid()
    {
        return handle > -1;
    }
}
public class THPhysicsObject : THObject
{
    private int currentHandle = -1;
    private int gravityForceHandle;
    private List<THAppliedForce> appliedForces = new();
    
    private int frictionForceHandle;


    private FVector2 currentVelocity;
    
    public THPhysicsObject(FVector2 position) : base(position)
    {
        gravityForceHandle = GenerateAppliedForce(new FVector2(FP.Zero, THStatics.GM.gameRules.gravityForce), AppliedForceTypes.Force);
        frictionForceHandle = GenerateAppliedForce(FVector2.Zero, AppliedForceTypes.Force);
    }

    public int GenerateAppliedForce(FVector2 appliedForce, AppliedForceTypes appliedForceType)
    {
        if (appliedForce == FVector2.Zero && appliedForceType == AppliedForceTypes.Impulse)
        {
            return -1;
        }
        
        currentHandle++;
        int nextHandle = currentHandle;
        THAppliedForce newForce = new THAppliedForce(nextHandle, appliedForce, appliedForceType);
        appliedForces.Add(newForce);
        return currentHandle;
    }

    public void ApplyAllForces(int deltaFrames, FVector2 clampVelocityMagnitudes)
    {
        FVector2 allImpulses = FVector2.Zero;
        FVector2 allForces = FVector2.Zero;
        
        UpdateEnvironmentalForces();
        for (int i = 0; i < appliedForces.Count; i++)
        {
            var force = appliedForces[i];
            if (!force.IsValid())
            {
                continue;
            }
            
            if (force.appliedForceType == AppliedForceTypes.Force)
            {
                allForces += force.appliedForce;
            }
            else if (force.appliedForceType == AppliedForceTypes.Impulse)
            {
                allImpulses += force.appliedForce;
                force.Invalidate();
                appliedForces[i] = force;
            }
        }
       
        FVector2 initialPosition = transform.position;
        currentVelocity += allImpulses;
        currentVelocity += allForces;
        currentVelocity = new FVector2(
            FMath.Clamp(currentVelocity.X, -clampVelocityMagnitudes.X, clampVelocityMagnitudes.X),
            FMath.Clamp(currentVelocity.Y, -clampVelocityMagnitudes.Y, clampVelocityMagnitudes.Y));

        if (FMath.Abs(currentVelocity.X) <= THStatics.GM.gameRules.velocityTooLowThreshold)
        {
            ZeroHorizontal();
        }

        FVector2 resPosition = initialPosition + currentVelocity * deltaFrames;
        
        MoveToLocation(resPosition);

        appliedForces.RemoveAll(RemoveAllInvalids);
    }

    private void UpdateEnvironmentalForces()
    {
        if (currentVelocity.X > FP.Zero)
        {
            ChangeAppliedForce(frictionForceHandle, new FVector2(-THStatics.GM.gameRules.frictionForce, FP.Zero));
        }
        else if (currentVelocity.X < FP.Zero)
        {
            ChangeAppliedForce(frictionForceHandle, new FVector2(THStatics.GM.gameRules.frictionForce, FP.Zero));
        }
        else
        {
            ChangeAppliedForce(frictionForceHandle, FVector2.Zero);
        }
    }

    private void ChangeAppliedForce(int handle, FVector2 newAppliedForce)
    {
        int index = GetIndexOfAppliedForce(frictionForceHandle);
        if (index >= 0 && appliedForces.Count > index)
        {
            THAppliedForce force = appliedForces[index];
            appliedForces[index] = new THAppliedForce(force.handle, newAppliedForce, force.appliedForceType);
        }
    }

    private int GetIndexOfAppliedForce(int handle)
    {
        for (int i = 0; i < appliedForces.Count; i++)
        {
            if (appliedForces[i].handle == handle)
            {
                return i;
            }
        }
        
        return -1;
    }

    private bool RemoveAllInvalids(THAppliedForce force)
    {
        return !force.IsValid();
    }

    public void ZeroHorizontal()
    {
        currentVelocity = new FVector2(FP.Zero, currentVelocity.Y);
        ChangeAppliedForce(frictionForceHandle, FVector2.Zero);
    }
}