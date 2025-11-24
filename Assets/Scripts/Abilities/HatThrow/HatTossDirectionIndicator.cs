using System;
using FMODUnity;
using UnityEngine;

public class HatTossDirectionIndicator : MonoBehaviour
{
    [SerializeField] private Transform rotationRoot;

    private StudioEventEmitter tossHoldLoop;

    private void Start()
    {
        tossHoldLoop = GameWizard.Instance.audioHub.SetupLoopingClip(Help.Audio.hatTossHold);
        GameWizard.Instance.audioHub.PlayLoopingClip(tossHoldLoop);
    }

    public void UpdateArrowRotation(float newAngle)
    {
        rotationRoot.localRotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
    }

    private void OnDisable()
    {
        GameWizard.Instance.audioHub.DestroyLoopingClip(tossHoldLoop);
    }
}