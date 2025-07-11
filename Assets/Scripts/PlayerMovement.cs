﻿using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float verticalJumpPower = 8f;
    [SerializeField] private float horizontalJumpPower = 2f;
    [SerializeField] private float risingGravity = 2f;
    [SerializeField] private float fallingGravity = 7f;
    
    private InputHandler inputHandler;
    private float currentXVal = 0;
    private Player player;
    private bool isJumping = false;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        inputHandler.OnHorizontalInputChanged += OnHorizontalInputChanged;
        inputHandler.OnVerticalInputChanged += OnVerticalInputChanged;
        player = GetComponent<Player>();
        rb2d = GetComponent<Rigidbody2D>();
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
        if (player.CanMove())
        {
            transform.position += new Vector3(GetWalkSpeed() * currentXVal * Time.deltaTime, 0, 0);
        }
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
        foreach (var spawnedFist in player.GetSpawnedFists())
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
        currentXVal = value;
    }

    private void OnVerticalInputChanged(float value)
    {
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
        if (!IsJumping())
        {
            StartJump();
        }
    }
}