using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WingsAbility : MonoBehaviour
{
    //[Header("HighJump")]
    //[SerializeField] private float highJumpForce = 2.1f;            //���� ������ ����� �������������
    //private bool glideStatus = true;                                //������ ������������

    //[Header("Glide")]
    //[SerializeField] private float fallingSpeed = 5f;               //�������� ������� ��������� �� ����� ������������
    //[SerializeField] private float glindingSpeed = 8f;              //�������� ��������� �� ����� ������������
    //private float initialGravityScale;                              //�������������� �������� ����������
    //public bool isGliding;                                         //��������� �� ��������

    //public Rigidbody2D rb;

    //private void Start()
    //{
    //    rb = GetComponent<Rigidbody2D>();

    //    initialGravityScale = rb.gravityScale;
    //}

    ////������� ������
    //#region HighJump Function
    //public void OnHighJump(InputAction.CallbackContext context)
    //{
    //    if (glideStatus && context.started && isGrounded())   //���� �������� �� �����, ������ ������ ����������� � ������� ������
    //    {
    //        glideStatus = false;
    //        moveSpeed = 0f;
    //    }
    //    if (context.performed && isGrounded())                //���� �������� �� ����� � ������� ���������
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x, jumpForce * highJumpForce);
    //        moveSpeed = speedController;
    //    }
    //    else if (context.canceled)                                      //���� ������� ���������
    //    {
    //        glideStatus = true;
    //        moveSpeed = speedController;
    //    }
    //}
    //#endregion

    ////������������
    //#region Glide Function
    //public void OnGlide(InputAction.CallbackContext context)
    //{
    //    if (context.performed && rb.velocity.y < 0 && !isGrounded())  //���� ������� ��������� � �������� ������
    //    {
    //        isGliding = true;
    //        rb.gravityScale = 0;
    //        moveSpeed = glindingSpeed;
    //        rb.velocity = new Vector2(rb.velocity.x, -fallingSpeed);
    //    }
    //    else                                            //����� ������� �� ������������
    //    {
    //        isGliding = false;
    //        moveSpeed = speedController;
    //        rb.gravityScale = initialGravityScale;
    //    }
    //}
    //#endregion



}
