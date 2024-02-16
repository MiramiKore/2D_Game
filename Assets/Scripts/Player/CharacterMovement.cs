using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{  
    [Header("Movement")]
    [HideInInspector] public bool isFacingRight = true;             //положение поворота персонажа
    [SerializeField] private float moveSpeed = 12f;                 //скорость перемещения
    private float speedController;                                  //константа скорости
    private Vector2 _moveDirection;

    [Header("Dash")]
    [SerializeField] private float dashImpulse;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;                 //сила прыжка
    [SerializeField] private float doubleJumpForce = 0.8f;          //сила второго прыжка
    private bool doubleJump;                                        //возможность второго прыжка

    [Header("Glide")]
    [SerializeField] private float fallingSpeed = 5f;               //скорость падения персонажа во время планирования
    [SerializeField] private float glindingSpeed = 0.5f;            //скорость персонажа во время планирования
    [SerializeField] private float jumpForceBeforeGlide = 2.1f;     //сила прыжка перед планированием
    private bool glideStatus = true;                                //статус планирования
    private float initialGravityScale;                              //первоначальное значение гравитации

    //Компоненты игрового объекта
    static public Rigidbody2D rb;
    public PlayerInput playerInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        initialGravityScale = rb.gravityScale;

        speedController = moveSpeed;
    }

    private void Update()
    {
        Move(_moveDirection);
        Timer.TimeRefresh();
        ObjectChecker.Checker();
    }

    #region Movement Function
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    public void Move(Vector2 direction)
    {
        if (_moveDirection.x > 0 || _moveDirection.x < 0)
        {
            TurnCheck();
        }

        rb.velocity = new Vector2(_moveDirection.x * moveSpeed, rb.velocity.y);
    }
    #endregion

    #region Turn Function
    private void TurnCheck()
    {
        if (_moveDirection.x > 0 && !isFacingRight)
        {
            Turn();
        }
        else if (_moveDirection.x < 0 && isFacingRight)
        {
            Turn();
        }
    }

    private void Turn()
    {
        if (isFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
        }
    }
    #endregion

    public void OnJump(InputAction.CallbackContext context)
    {
        if (ObjectChecker.isGround && context.performed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            doubleJump = true;
        }
        else if (!ObjectChecker.isGround && context.performed && doubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
            doubleJump = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {



    }

    public void OnHighJump(InputAction.CallbackContext context)
    {
        if (ObjectChecker.isGround && glideStatus && context.started)
        {
            glideStatus = false;
            moveSpeed = 0f;
        }
        if (ObjectChecker.isGround && context.performed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * jumpForceBeforeGlide);
            moveSpeed = speedController;
        }
        else if (context.canceled)
        {
            glideStatus = true;
            moveSpeed = speedController;
        }
    }

    public void OnGlide(InputAction.CallbackContext context)
    {
        if (rb.velocity.y <= 0 && !ObjectChecker.isGround && context.performed)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x * glindingSpeed, -fallingSpeed);
        }
        else
        {
            rb.gravityScale = initialGravityScale;
        }
    }
}