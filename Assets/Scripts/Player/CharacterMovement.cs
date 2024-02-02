using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;             //скорость перемещения
    [SerializeField] private float speedController;             //константа скорости

    [Header("Dash")]
    [SerializeField] private int dashImpulse = 6000;            //сила рывка (dash)
    [SerializeField] private bool dashInAir;
    [SerializeField] private float dashTime;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;             //сила прыжка
    [SerializeField] private float doubleJumpForce = 0.8f;      //сила второго прыжка
    [SerializeField] private bool doubleJump;                   //возможность второго прыжка

    [Header("Gliding")]
    [SerializeField] private float fallingSpeed= 5f;            //скорость падения персонажа во время планирования
    [SerializeField] private float glindingSpeed = 0.5f;        //скорость персонажа во время планирования
    [SerializeField] private float glideCheckTimer;             //таймер планирования
    [SerializeField] private float jumpForceBeforeGlide = 2.1f; //сила прыжка перед планированием
    [SerializeField] private bool glideStatus;                  //статус планирования
    [SerializeField] private bool glidingMode;                  //режим планирования

    [Header("Camera Stuff")]
    [SerializeField] private GameObject cameraFollowGo;         //следование камеры за персонажем

    [HideInInspector] public bool isFacingRight;                //положение поворота персонажа

    static public Rigidbody2D rb;                           
    private float _moveInput;            //перемещение  (кнопка нажата)
    private bool _jumpInput;             //прыжок       (кнопка нажата)
    private bool _dashInput;             //рывок        (кнопка нажата)
    private bool _glideInput;            //планирование (кнопка нажата)
    private bool _glideInputPressed;     //планирование (кнопка удерживается)
    private bool _glideInputReleased;    //планирование (кнопка отпущена)

    private bool isJumping;              //хранение значения прыгнул ли персонаж        
    private bool isFalling;              //хранение значения падает ли персонаж

    private float initialGravityScale;   //первоначальное значение гравитации

    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeThreshold;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        initialGravityScale = rb.gravityScale;

        cameraFollowObject = cameraFollowGo.GetComponent<CameraFollowObject>();

        fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;

        speedController = moveSpeed;
    }

    private void Update()
    {
        Move();
        Jump();
        Dash();
        Gliding();
        CameraFollow();
        Timer.TimeRefresh();
        ObjectChecker.Checker();
    }

    #region Movement Function
    private void Move()
    {
        _moveInput = UserInput.instance.moveInput.x;

        if (_moveInput > 0 || _moveInput < 0)
        {
            TurnCheck();
        }

        rb.velocity = new Vector2(_moveInput * moveSpeed, rb.velocity.y);
    }
    #endregion

    #region Turn Function
    private void TurnCheck()
    {
        if (UserInput.instance.moveInput.x > 0 && !isFacingRight)
        {
            Turn();
        }
        else if (UserInput.instance.moveInput.x < 0 && isFacingRight)
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

            cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;

            cameraFollowObject.CallTurn();
        }
    }
    #endregion

    #region Jump Function
    void Jump()
    {
        _jumpInput = UserInput.instance.controls.Jumping.Jump.WasPressedThisFrame();

        if (_jumpInput)
        {
            //первый прыжок
            if (ObjectChecker.isGround)
            {
                isJumping = true;
                doubleJump = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Timer.jumpTimer = 0.7f;
            }
            //второй прыжок
            else if(doubleJump && !ObjectChecker.isGround && Timer.jumpTimer >= 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * doubleJumpForce);
                doubleJump = false;
            }
        }
        //проверка наличия второго прыжка
        if (Timer.jumpTimer < 0 && ObjectChecker.isGround)
        {
            doubleJump = false;
            isJumping = false;
        }
    }
    #endregion

    #region Dash Function
    void Dash()
    {
        _dashInput = UserInput.instance.controls.Dashing.Dash.WasPressedThisFrame();
        dashTime = Timer.dashTimer;

        if (_dashInput && Timer.dashTimer <= 0)
        {
            dashInAir = true;
            rb.velocity = new Vector2(0, 0);
            Timer.dashTimer = 1f;

            if (!isFacingRight && !ObjectChecker.isWallLeft)
            {
                rb.AddForce(Vector2.left * dashImpulse);
            }

            if (isFacingRight && !ObjectChecker.isWallRight)
            {
                rb.AddForce(Vector2.right * dashImpulse);
            }
        }

        if (!ObjectChecker.isGround && _dashInput && dashInAir)
        {
            Timer.dashTimer = 2f;
            dashInAir = false;
        }
    }
    #endregion

    #region Gliding Function
    void Gliding()
    {
        glideCheckTimer = Timer.glideTimer;

        _glideInput = UserInput.instance.controls.Gliding.Glide.WasPressedThisFrame();
        _glideInputPressed = UserInput.instance.controls.Gliding.Glide.IsPressed();
        _glideInputReleased = UserInput.instance.controls.Gliding.Glide.WasReleasedThisFrame();

        //переход в режим планирования (подготовка)
        if (ObjectChecker.isGround && glideStatus && _glideInput)
        {
            glidingMode = true;
            glideStatus = false;
            Timer.glideTimer = 2f;
            moveSpeed = 0f;
        }
        else if (_glideInputReleased)
        {
            glidingMode = false;
            glideStatus = true;
            moveSpeed = speedController;
            Timer.glideTimer = 0f;
        }
        //прыжок перед планированием
        if (Timer.glideTimer <= 0f && ObjectChecker.isGround && glidingMode && _glideInputPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * jumpForceBeforeGlide);
        }
        //планирование
        if (rb.velocity.y <= 0 && !ObjectChecker.isGround && _glideInputPressed)
        {
            moveSpeed = speedController;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x * glindingSpeed, -fallingSpeed);
            Timer.glideTimer = 2f;
        }
        else
        {
            rb.gravityScale = initialGravityScale;
        }
    }
    #endregion

    #region CameraFollow Function
    private void CameraFollow()
    {
        if (rb.velocity.y < fallSpeedYDampingChangeThreshold && !CameraManager.instance.isLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (rb.velocity.y >= 0f && !CameraManager.instance.isLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }
    }
    #endregion
}