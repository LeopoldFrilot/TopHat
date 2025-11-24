using System;
using UnityEngine;

public class ThrownHat : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Collider2D mainCollider;

    private HatStats hatStats;
    private Rigidbody2D rigidbody2D;
    private Fighter ownerFighter;
    private Vector3 lastLocation;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (lastLocation != Vector3.zero)
        {
            Vector3 direction = (transform.position - lastLocation).normalized;
            float angle = Quaternion.Angle(Quaternion.LookRotation(Vector3.right, Vector3.forward),
                Quaternion.LookRotation(direction, Vector3.forward));
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, -angle - 90)) ;
            renderer.transform.localRotation = rotation;
        }
        
        lastLocation = transform.position;
    }

    public void Initialize(HatStats inHatStats, Fighter inOwnerFighter)
    {
        hatStats = inHatStats;
        ownerFighter = inOwnerFighter;
        renderer.sprite = hatStats.hatIcon;
        Physics2D.IgnoreCollision(mainCollider, ownerFighter.GetMainCollider());
    }

    public void Launch(Vector3 direction, float strength)
    {
        rigidbody2D.AddForce(direction.normalized * strength, ForceMode2D.Impulse);
    }

    public Action<Fighter> OnFighterTriggerEntered;
    private void OnCollisionEnter2D(Collision2D other)
    {
        var hitFighter = other.collider.transform.root.GetComponent<Fighter>();
        if (hitFighter)
        {
            OnFighterTriggerEntered.Invoke(hitFighter);
        }
        else
        {
            GameWizard.Instance.audioHub.PlayClip(Help.Audio.hatTossBounce);
        }
    }
}