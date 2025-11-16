using System;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float verticalJumpPower = 8f;
    [SerializeField] private float horizontalJumpPower = 2f;
    [SerializeField] private float risingGravity = 2f;
    [SerializeField] private float fallingGravity = 7f;
    
    private InputHandler inputHandler;
    private float currentXVal = 0;
    private Fighter fighter;
    private bool isJumping = false;
    private Rigidbody2D rb2d;

    private Vector3 lastPosition;
    private Vector2 currentSpeed;

    private void Awake()
    {
        fighter = GetComponent<Fighter>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (fighter.IsInitioalized())
        {
            Initialize();
        }
        else
        {
            fighter.OnInitialized += Initialize;
        }
    }

    private void Initialize()
    {
        inputHandler = fighter.GetInputHandler();
        inputHandler.OnHorizontalInputChanged += OnHorizontalInputChanged;
    }

    public void RegisterHorizontalInput(float value)
    {
        OnHorizontalInputChanged(value, fighter.networkedFighterController.GetPlayerIndex(fighter));
    }

    private void Update()
    {
        if (fighter.CanMove())
        {
            Vector3 newPosition = transform.position + new Vector3(GetWalkSpeed() * currentXVal * Time.deltaTime, 0, 0);
            transform.position = fighter.ClampToFightPosition(newPosition);
        }

        Vector3 difference = transform.position - lastPosition;
        currentSpeed = new Vector2(difference.x / Time.deltaTime, difference.y / Time.deltaTime);
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (IsJumping())
        {
            rb2d.gravityScale = rb2d.linearVelocity.y < 0 ? fallingGravity : risingGravity;
        }
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    private float GetWalkSpeed()
    {
        if (IsJumping())
        {
            return 0;
        }
        
        int fistsInMotion = 0;
        foreach (var spawnedFist in fighter.GetSpawnedFists())
        {
            if (spawnedFist.GetCurrentState() != PlayerFistState.Idle)
            {
                fistsInMotion++;
            }
        }
        
        return speed * Mathf.Pow(.6f, fistsInMotion);
    }

    private void OnHorizontalInputChanged(float value, int playerOnNetworkedController)
    {
        if (playerOnNetworkedController != fighter.networkedFighterController.GetPlayerIndex(fighter))
        {
            return;
        }

        currentXVal = value;
    }

    private void StartJump()
    {
        LaunchPlayer(new Vector2(currentXVal * horizontalJumpPower, verticalJumpPower));
    }

    public void Land()
    {
        isJumping = false;
        rb2d.gravityScale = risingGravity;
        rb2d.totalForce = Vector2.zero;
        rb2d.linearVelocity = Vector2.zero;
    }

    public void LaunchPlayer(Vector2 power)
    {
        if (power.y > 0)
        {
            isJumping = true;
        }
        
        rb2d.AddForce(power, ForceMode2D.Impulse);
    }

    public void Jump()
    {
        if (CanJump())
        {
            StartJump();
        }
    }

    private bool CanJump()
    {
        return !IsJumping() && fighter.CanMove();
    }

    public float GetHorizontalVelocity()
    {
        return currentSpeed.x;
    }

    public bool IsHoldingLeft()
    {
        return currentXVal < 0;
    }
}