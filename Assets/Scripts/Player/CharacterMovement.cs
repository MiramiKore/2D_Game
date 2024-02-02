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
    private float speedController;                              //��������� ��������

    [Header("Dash")]
    [SerializeField] private int dashImpulse = 6000;            //���� ����� (dash)
    private bool dashInAir;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;             //���� ������
    [SerializeField] private float doubleJumpForce = 0.8f;      //���� ������� ������
    private bool doubleJump;                                    //����������� ������� ������

    [Header("Gliding")]
    [SerializeField] private float fallingSpeed= 5f;            //�������� ������� ��������� �� ����� ������������
    [SerializeField] private float glindingSpeed = 0.5f;        //�������� ��������� �� ����� ������������
    [SerializeField] private float jumpForceBeforeGlide = 2.1f; //���� ������ ����� �������������
    private bool glideStatus = true;                            //������ ������������
    private bool glidingMode;                                   //����� ������������

    [Header("Camera Stuff")]
    [SerializeField] private GameObject cameraFollowGo;         //���������� ������ �� ����������

    [HideInInspector] public bool isFacingRight = true;         //��������� �������� ���������

    static public Rigidbody2D rb;                           

    private float initialGravityScale;                          //�������������� �������� ����������

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
        if (UserInput.instance.moveInput.x > 0 || UserInput.instance.moveInput.x < 0)
        {
            TurnCheck();
        }

        rb.velocity = new Vector2(UserInput.instance.moveInput.x * moveSpeed, rb.velocity.y);
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
        if (UserInput.instance.controls.Player.Jump.WasPressedThisFrame())
        {
            //������ ������
            if (ObjectChecker.isGround)
            {
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
        }
    }
    #endregion

    #region Dash Function
    void Dash()
    {
        if (UserInput.instance.controls.Player.Dash.WasPressedThisFrame() && Timer.dashTimer <= 0)
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

        if (!ObjectChecker.isGround && UserInput.instance.controls.Player.Dash.WasPressedThisFrame() && dashInAir)
        {
            Timer.dashTimer = 2f;
            dashInAir = false;
        }
    }
    #endregion

    #region Gliding Function
    void Gliding()
    {
        //������� � ����� ������������ (����������)
        if (ObjectChecker.isGround && glideStatus && UserInput.instance.controls.Player.Glide.WasPressedThisFrame())
        {
            glidingMode = true;
            glideStatus = false;
            Timer.glideTimer = 2f;
            moveSpeed = 0f;
        }
        //������� ������ ���� ��������
        else if (UserInput.instance.controls.Player.Glide.WasReleasedThisFrame())
        {
            glidingMode = false;
            glideStatus = true;
            moveSpeed = speedController;
            Timer.glideTimer = 0f;
        }
        //������ ����� �������������
        if (Timer.glideTimer <= 0f && ObjectChecker.isGround && glidingMode && UserInput.instance.controls.Player.Glide.IsPressed())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * jumpForceBeforeGlide);
        }
        //������������
        if (rb.velocity.y <= 0 && !ObjectChecker.isGround && UserInput.instance.controls.Player.Glide.IsPressed())
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