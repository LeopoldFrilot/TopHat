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
    [SerializeField] private Collider2D collider;
    
    private Transform targetPos;
    private Tweener idleFollow;
    private bool inTransition = false;
    private Coroutine transitionCoroutine;
    private Rigidbody2D hatRigidBody;

    private void Awake()
    {
        hatRigidBody = GetComponent<Rigidbody2D>();
    }

    public void SetNewTarget(Transform newTarget)
    {
        if (targetPos != newTarget)
        {
            targetPos = newTarget;
            inTransition = newTarget != null;
            
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }

            if (idleFollow != null)
            {
                idleFollow.Kill();
            }

            if (inTransition)
            {
                transitionCoroutine = StartCoroutine(MoveInArc(transform, targetPos, transitionTime, transitionArcHeight));
            }
        }
    }

    private void OnReachedTarget()
    {
        inTransition = false;
        if (targetPos)
        {
            idleFollow = transform.DOMove(targetPos.position, idleFollowSpeed).SetSpeedBased(true).SetEase(Ease.OutCubic);
            idleFollow.OnUpdate(() => idleFollow.ChangeEndValue(targetPos.position, true).Restart());
        }
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

    public void PausePhysics()
    {
        hatRigidBody.bodyType = RigidbodyType2D.Static;
        collider.enabled = false;
    }

    public void ResumePhysics()
    {
        hatRigidBody.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
    }

    public Action<Fighter> OnHatCollected;
    private void OnCollisionEnter2D(Collision2D other)
    {
        Fighter fighter = other.transform.root.GetComponent<Fighter>();
        if (fighter != null)
        {
            PausePhysics();
            OnHatCollected?.Invoke(fighter);
        }
    }
}