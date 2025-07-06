using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHat : MonoBehaviour
{
    [SerializeField] private float idleFollowSpeed = 4f;
    [SerializeField] private float transitionTime = .5f;
    [SerializeField] private float transitionArcHeight = 3f;
    [SerializeField] private float hatSpinSpeed = 100f;
    
    private Transform targetPos;
    private Tweener idleFollow;
    private bool inTransition = false;
    private Coroutine transitionCoroutine;

    public void SetNewTarget(Transform newTarget)
    {
        if (targetPos != newTarget)
        {
            inTransition = true;
            targetPos = newTarget;
            
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            
            transitionCoroutine = StartCoroutine(MoveInArc(transform, targetPos, transitionTime, transitionArcHeight));
        }
    }

    private void OnReachedTarget()
    {
        inTransition = false;
        idleFollow = transform.DOMove(targetPos.position, idleFollowSpeed).SetSpeedBased(true).SetEase(Ease.OutCubic);
        idleFollow.OnUpdate(() => idleFollow.ChangeEndValue(targetPos.position, true).Restart());
    }

    public bool IsInTransition()
    {
        return inTransition;
    }
    
    private IEnumerator MoveInArc(Transform mover, Transform target, float duration, float arcHeight)
    {
        Vector3 start = mover.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mover.localEulerAngles += new Vector3(0,0,Time.deltaTime * hatSpinSpeed);
            float t = Mathf.Clamp01(elapsed / duration);

            // Get updated target position
            Vector3 end = target.position;

            // Linear interpolation
            Vector3 linearPos = Vector3.Lerp(start, end, t);

            // Add arc offset (simple parabolic arc using sine)
            float heightOffset = Mathf.Sin(t * Mathf.PI) * arcHeight;
            Vector3 arcPos = linearPos + Vector3.up * heightOffset;

            mover.position = arcPos;
            yield return null;
        }
        
        mover.localRotation = Quaternion.identity;
        mover.position = target.position;
        transitionCoroutine = null;
        OnReachedTarget();
    }
}