using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WingsAbility : MonoBehaviour
{
    //[Header("HighJump")]
    //[SerializeField] private float highJumpForce = 2.1f;            //сила прыжка перед планированием
    //private bool glideStatus = true;                                //статус планирования

    //[Header("Glide")]
    //[SerializeField] private float fallingSpeed = 5f;               //скорость падения персонажа во время планирования
    //[SerializeField] private float glindingSpeed = 8f;              //скорость персонажа во время планирования
    //private float initialGravityScale;                              //первоначальное значение гравитации
    //public bool isGliding;                                         //планирует ли персонаж

    //public Rigidbody2D rb;

    //private void Start()
    //{
    //    rb = GetComponent<Rigidbody2D>();

    //    initialGravityScale = rb.gravityScale;
    //}

    ////Высокий прыжок
    //#region HighJump Function
    //public void OnHighJump(InputAction.CallbackContext context)
    //{
    //    if (glideStatus && context.started && isGrounded())   //если персонаж на земле, статус прыжка активирован и клавиша нажата
    //    {
    //        glideStatus = false;
    //        moveSpeed = 0f;
    //    }
    //    if (context.performed && isGrounded())                //если персонаж на земле и клавиша сработала
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x, jumpForce * highJumpForce);
    //        moveSpeed = speedController;
    //    }
    //    else if (context.canceled)                                      //если клавиша отпущенна
    //    {
    //        glideStatus = true;
    //        moveSpeed = speedController;
    //    }
    //}
    //#endregion

    ////Планирование
    //#region Glide Function
    //public void OnGlide(InputAction.CallbackContext context)
    //{
    //    if (context.performed && rb.velocity.y < 0 && !isGrounded())  //если клавиша сработала и персонаж падает
    //    {
    //        isGliding = true;
    //        rb.gravityScale = 0;
    //        moveSpeed = glindingSpeed;
    //        rb.velocity = new Vector2(rb.velocity.x, -fallingSpeed);
    //    }
    //    else                                            //иначе выходим из планирования
    //    {
    //        isGliding = false;
    //        moveSpeed = speedController;
    //        rb.gravityScale = initialGravityScale;
    //    }
    //}
    //#endregion



}
