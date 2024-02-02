using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovedFunctionality //��������� ����� ����
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;         //�������� �����������

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;          //���� ������
    [SerializeField] private float jumpTime = 0.5f;         //������������ �������� ������ (��� ������ �������� - ��� ������� ����� ������ �������)
    [SerializeField] private float jumpTimeCounter;         //������� ������� �������� ������ (��������� jumpTime �� 0)

    [Header("Dash")]
    [SerializeField] private int dashImpulse = 6000;        // ���� ����� (dash)

    [Header("Gliding")]
    [SerializeField] private float fallingSpeed = 8f;        //�������� ������� ��������� �� ����� ������������
    [SerializeField] private float glindingSpeed = 0.5f;    //�������� ��������� �� ����� ������������
    [SerializeField] private bool glidingMode;              //������ �������� (���������� �� �������� � ������)

    [Header("Camera Stuff")]
    [SerializeField] private GameObject cameraFollowGo;

    [HideInInspector] public bool isFacingRight;            //��������� ���������

    static public Rigidbody2D rb;
    private float moveInput;                                //���� ������ ��� ����������� �� �����
    private bool glidingInput;                              //���� ������ ��� ����������� � ������������

    private bool isJumping;                                 //������ �������� ������� �� ��������        
    private bool isFalling;                                 //������ �������� ������ �� ��������


    private float initialGravityScale;                      //�������������� �������� ����������

    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeThreshold;

    void Jump()
    {
        //������� ������
        if (UserInput.instance.controls.Jumping.Jump.WasPressedThisFrame() && ObjectChecker.isGround)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        //������� ������������
        if (UserInput.instance.controls.Jumping.Jump.IsPressed())
        {
            if (jumpTimeCounter > 0 && isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        //������� ��������
        if (UserInput.instance.controls.Jumping.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
        }
    }









}
