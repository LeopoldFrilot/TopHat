using System;
using FMODUnity;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private bool valid = true;
    private StudioEventEmitter emitter;

    public void Initialize(StudioEventEmitter emitter)
    {
        this.emitter = emitter;
    }

    private void Update()
    {
        if (!valid)
        {
            if (emitter.EventReference.IsNull || !emitter.IsPlaying())
            {
                Destroy(gameObject);
            }
        }
    }

    public void DestroyPlayer()
    {
        emitter.Stop();
        valid = false;
    }
}