using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;             //�������� �����������
    [SerializeField] private float speedController;             //��������� ��������

    [Header("Dash")]
    [SerializeField] private int dashImpulse = 6000;            //���� ����� (dash)
    [SerializeField] private bool dashInAir;
    [SerializeField] private float dashTime;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;             //���� ������
    [SerializeField] private float doubleJumpForce = 0.8f;      //���� ������� ������
    [SerializeField] private bool doubleJump;                   //����������� ������� ������

    [Header("Gliding")]
    [SerializeField] private float fallingSpeed= 5f;            //�������� ������� ��������� �� ����� ������������
    [SerializeField] private float glindingSpeed = 0.5f;        //�������� ��������� �� ����� ������������
    [SerializeField] private float glideCheckTimer;             //������ ������������
    [SerializeField] private float jumpForceBeforeGlide = 2.1f; //���� ������ ����� �������������
    [SerializeField] private bool glideStatus;                  //������ ������������
    [SerializeField] private bool glidingMode;                  //����� ������������

    [Header("Camera Stuff")]
    [SerializeField] private GameObject cameraFollowGo;         //���������� ������ �� ����������

    [HideInInspector] public bool isFacingRight;                //��������� �������� ���������

    static public Rigidbody2D rb;                           
    private float _moveInput;            //�����������  (������ ������)
    private bool _jumpInput;             //������       (������ ������)
    private bool _dashInput;             //�����        (������ ������)
    private bool _glideInput;            //������������ (������ ������)
    private bool _glideInputPressed;     //������������ (������ ������������)
    private bool _glideInputReleased;    //������������ (������ ��������)

    private bool isJumping;              //�������� �������� ������� �� ��������        
    private bool isFalling;              //�������� �������� ������ �� ��������

    private float initialGravityScale;   //�������������� �������� ����������

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
            //������ ������
            if (ObjectChecker.isGround)
            {
                isJumping = true;
                doubleJump = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Timer.jumpTimer = 0.7f;
            }
            //������ ������
            else if(doubleJump && !ObjectChecker.isGround && Timer.jumpTimer >= 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * doubleJumpForce);
                doubleJump = false;
            }
        }
        //�������� ������� ������� ������
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

        //������� � ����� ������������ (����������)
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
        //������ ����� �������������
        if (Timer.glideTimer <= 0f && ObjectChecker.isGround && glidingMode && _glideInputPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * jumpForceBeforeGlide);
        }
        //������������
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