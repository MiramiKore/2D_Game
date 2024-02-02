using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovedFunctionality //Удаленные куски кода
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;         //скорость перемещения

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;          //сила прыжка
    [SerializeField] private float jumpTime = 0.5f;         //длительность высокого прыжка (чем меньше значение - тем быстрее будет тикать счетчик)
    [SerializeField] private float jumpTimeCounter;         //счетчик времени высокого прыжка (уменьшаем jumpTime до 0)

    [Header("Dash")]
    [SerializeField] private int dashImpulse = 6000;        // сила рывка (dash)

    [Header("Gliding")]
    [SerializeField] private float fallingSpeed = 8f;        //скорость падения персонажа во время планирования
    [SerializeField] private float glindingSpeed = 0.5f;    //скорость персонажа во время планирования
    [SerializeField] private bool glidingMode;              //храним значение (находиться ли персонаж в полете)

    [Header("Camera Stuff")]
    [SerializeField] private GameObject cameraFollowGo;

    [HideInInspector] public bool isFacingRight;            //положение персонажа

    static public Rigidbody2D rb;
    private float moveInput;                                //ввод данных для предвижения по земле
    private bool glidingInput;                              //ввод данных для предвижения в планировании

    private bool isJumping;                                 //храним значение прыгнул ли персонаж        
    private bool isFalling;                                 //храним значение падает ли персонаж


    private float initialGravityScale;                      //первоначальное значение гравитации

    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeThreshold;

    void Jump()
    {
        //Клавиша нажата
        if (UserInput.instance.controls.Jumping.Jump.WasPressedThisFrame() && ObjectChecker.isGround)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        //Клавиша удерживается
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
        //Клавиша отпущена
        if (UserInput.instance.controls.Jumping.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
        }
    }









}
