using System;
using ScriptableObjects;
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
            float angle = Mathf.Atan(direction.y/direction.x) * Mathf.Rad2Deg;
            renderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)) ;
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
    }
}