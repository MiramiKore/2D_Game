using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{  
    [Header("Movement")]
    [HideInInspector] public bool isFacingRight = true;             //��������� �������� ���������
    [SerializeField] private float moveSpeed = 12f;                 //�������� �����������
    private float speedController;                                  //��������� ��������
    private Vector2 _moveDirection;                                 //������ ��������

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;                 //���� ������
    [SerializeField] private float doubleJumpForce = 0.8f;          //���� ������� ������
    private bool doubleJump;                                        //����������� ������� ������

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;                 //�������� �����
    [SerializeField] private float dashDuration = 0.1f;             //������������ �����
    [SerializeField] private float dashCooldown = 0.1f;             //����������� �����
    [SerializeField] private float dashCooldownInGlide = 0.1f;      //����������� ����� ��� ������������
    private bool canDash = true;                                    //����� �� �������� ������� �����
    private bool isDashing;                                         //������ �� �������� �����
    
    [Header("HighJump")]
    [SerializeField] private float highJumpForce = 2.1f;            //���� ������ ����� �������������
    private bool glideStatus = true;                                //������ ������������

    [Header("Glide")]
    [SerializeField] private float fallingSpeed = 5f;               //�������� ������� ��������� �� ����� ������������
    [SerializeField] private float glindingSpeed = 8f;              //�������� ��������� �� ����� ������������
    private float initialGravityScale;                              //�������������� �������� ����������
    private bool isGliding;                                         //��������� �� ��������

    //���������� �������� �������
    static public Rigidbody2D rb;
    public PlayerInput playerInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        initialGravityScale = rb.gravityScale;  //�������� �������������� ���� ����������

        speedController = moveSpeed;            //�������� �������������� �������� ��������
    }

    private void Update()
    {
        if (isDashing)                          //��������� ����������� �������� ��� �����
        {
            return;
        }

        Move(_moveDirection);
        ObjectChecker.Checker();
    }
    
    //������������
    #region Movement Function
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    public void Move(Vector2 direction)
    {
        if (_moveDirection.x > 0 || _moveDirection.x < 0)   //������������ ��������� � ����������� �� ����������� �������� (�����-����)
        {
            TurnCheck();
        }

        rb.velocity = new Vector2(_moveDirection.x * moveSpeed, rb.velocity.y);
    }
    #endregion

    //������� ���������
    #region Turn Function
    private void TurnCheck()
    {
        if (_moveDirection.x > 0 && !isFacingRight)         //���� ��������� ������ - ������������ ���������
        {
            Turn();
        }
        else if (_moveDirection.x < 0 && isFacingRight)     //���� ��������� ����� - ������������ ���������
        {
            Turn();
        }
    }
    private void Turn()
    {
        Vector2 rotator;

        if (isFacingRight)  //���� ������� ������ � ��������� ����� - ��������������
        {
            rotator = new Vector2(transform.rotation.x, 180f);
        }
        else                //���� ������� ����� � ��������� ������ - ��������������
        {
            rotator = new Vector2(transform.rotation.x, 0f);
        }

        transform.rotation = Quaternion.Euler(rotator);
        isFacingRight = !isFacingRight;
    }
    #endregion

    //������
    #region Jump Function
    public void OnJump(InputAction.CallbackContext context)
    {
        if (ObjectChecker.isGround && context.performed)                        //���� ��������� �� ����� � ������� ������ - �������                    
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            doubleJump = true;
        }
        else if (!ObjectChecker.isGround && context.performed && doubleJump)    //�����, ���� ��������� � �������, ������� ������ � ���� ������ ������ - �������
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
            doubleJump = false;
        }
    }
    #endregion

    //�����
    #region Dash Function
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)   //���� ������� ��������� � ����� ������� ����� - �����
        {
            StartCoroutine(DashCoroutine());
        }
    }
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        float dashDirection = isFacingRight ? 1 : -1;               //�������� ����������� ���������

        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);              //������ ������������ �����

        isDashing = false;
        
        if (isGliding)  //���� �������� ��������� � ������������ - �������� ������� ��������� � �������� ������������            
        {
            rb.velocity = new Vector2(0f, -fallingSpeed);
            yield return new WaitForSeconds(dashCooldownInGlide);   //������ ����������� ��� ������������ 
        }
        else            //����� �������������� ���������� � �������������� ����� ��������
        {
            rb.gravityScale = initialGravityScale;
            rb.velocity = new Vector2(0f, rb.velocity.y);
            yield return new WaitForSeconds(dashCooldown);          //������ ����������� �� ����� � ��� ������
        }

        canDash = true;
    }
    #endregion

    //������� ������
    #region HighJump Function
    public void OnHighJump(InputAction.CallbackContext context)
    {
        if (ObjectChecker.isGround && glideStatus && context.started)   //���� �������� �� �����, ������ ������ ����������� � ������� ������
        {
            glideStatus = false;
            moveSpeed = 0f;
        }
        if (ObjectChecker.isGround && context.performed)                //���� �������� �� ����� � ������� ���������
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * highJumpForce);
            moveSpeed = speedController;
        }
        else if (context.canceled)                                      //���� ������� ���������
        {
            glideStatus = true;
            moveSpeed = speedController;
        }
    }
    #endregion

    //������������
    #region Glide Function
    public void OnGlide(InputAction.CallbackContext context)
    {
        if (context.performed && rb.velocity.y < 0)     //���� ������� ��������� � �������� ������
        {
            isGliding = true;
            rb.gravityScale = 0;
            moveSpeed = glindingSpeed;
            rb.velocity = new Vector2(rb.velocity.x, -fallingSpeed);
        }
        else                                            //����� ������� �� ������������
        {
            isGliding = false;
            moveSpeed = speedController;
            rb.gravityScale = initialGravityScale;
        }
    }
    #endregion
}