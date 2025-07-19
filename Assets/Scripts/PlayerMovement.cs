using System;
using DG.Tweening;
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
    private Fighter _fighter;
    private bool isJumping = false;
    private Rigidbody2D rb2d;

    private Vector3 lastPosition;
    private Vector2 currentSpeed;

    private void Awake()
    {
        _fighter = GetComponent<Fighter>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (_fighter.IsInitioalized())
        {
            Initialize();
        }
        else
        {
            _fighter.OnInitialized += Initialize;
        }
    }

    private void Initialize()
    {
        inputHandler = _fighter.GetInputHandler();
        inputHandler.OnHorizontalInputChanged += OnHorizontalInputChanged;
        inputHandler.OnVerticalInputChanged += OnVerticalInputChanged;
    }

    public void RegisterHorizontalInput(float value)
    {
        OnHorizontalInputChanged(value);
    }

    public void RegisterVerticalInput(float value)
    {
        OnVerticalInputChanged(value);
    }

    private void Update()
    {
        if (_fighter.CanMove())
        {
            transform.position += new Vector3(GetWalkSpeed() * currentXVal * Time.deltaTime, 0, 0);
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
        foreach (var spawnedFist in _fighter.GetSpawnedFists())
        {
            if (spawnedFist.GetCurrentState() != PlayerFistState.Idle)
            {
                fistsInMotion++;
            }
        }
        
        return speed * Mathf.Pow(.6f, fistsInMotion);
    }

    private void OnHorizontalInputChanged(float value)
    {
        if (!_fighter.CanMove())
        {
            value = 0;
        }

        currentXVal = value;
    }

    private void OnVerticalInputChanged(float value)
    {
        if (!_fighter.CanMove())
        {
            value = 0;
        }
        
        if (value > .75f)
        {
            Jump();
        }
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
        return !IsJumping() && _fighter.CanMove();
    }

    public float GetHorizontalVelocity()
    {
        return currentSpeed.x;
    }
}