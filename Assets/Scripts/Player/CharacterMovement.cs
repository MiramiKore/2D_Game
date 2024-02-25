using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class CharacterMovement : MonoBehaviour
{  
    [Header("Movement")]
    [HideInInspector] public bool isFacingRight = true;             //положение поворота персонажа
    [SerializeField] private float moveSpeed = 12f;                 //скорость перемещения
    private float speedController;                                  //константа скорости
    private Vector2 _moveDirection;                                 //вектор движения

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;                 //сила прыжка
    [SerializeField] private float doubleJumpForce = 0.8f;          //сила второго прыжка
    public bool doubleJump;                                        //возможность второго прыжка
    public float jumpDuration;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;                 //скорость рывка
    [SerializeField] private float dashDuration = 0.1f;             //длительность рывка
    [SerializeField] private float dashCooldown = 0.1f;             //перезарядка рывка
    [SerializeField] private float dashCooldownInGlide = 0.1f;      //перезарядка рывка при планировании
    private bool canDash = true;                                    //может ли персонаж сделать рывок
    private bool isDashing;                                         //делает ли персонаж рывок
    
    [Header("HighJump")]
    [SerializeField] private float highJumpForce = 2.1f;            //сила прыжка перед планированием
    private bool glideStatus = true;                                //статус планирования

    [Header("Glide")]
    [SerializeField] private float fallingSpeed = 5f;               //скорость падения персонажа во время планирования
    [SerializeField] private float glindingSpeed = 8f;              //скорость персонажа во время планирования
    private float initialGravityScale;                              //первоначальное значение гравитации
    public bool isGliding;                                         //планирует ли персонаж

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    //Компоненты игрового объекта
    static public Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        initialGravityScale = rb.gravityScale;  //хранение первоначальной силы гравитации

        speedController = moveSpeed;            //хранение первоначальной скорость перонажа
    }

    private void Update()
    {
        if (isDashing)                          //блокируем направление движения при рывке
        {
            return;
        }

        Move(_moveDirection);
    }
    public bool isGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
    }

    //Передвижение
    #region Movement Function
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    public void Move(Vector2 direction)
    {
        if (_moveDirection.x > 0 || _moveDirection.x < 0)   //поворачиваем персонажа в зависимости от направления движения (право-лево)
        {
            TurnCheck();
            Animations.move = true;
        }
        else
        {
            Animations.move = false;
        }

        rb.velocity = new Vector2(_moveDirection.x * moveSpeed, rb.velocity.y);


    }
    #endregion

    //Поворот персонажа
    #region Turn Function
    private void TurnCheck()
    {
        if (_moveDirection.x > 0 && !isFacingRight)         //если двигаемся вправо - поворачиваем персонажа
        {
            Turn();
        }
        else if (_moveDirection.x < 0 && isFacingRight)     //если двигаемся влево - поворачиваем персонажа
        {
            Turn();
        }
    }
    private void Turn()
    {
        Vector2 rotator;

        if (isFacingRight)  //если смотрим вправо и двигаемся влево - поворачиваемся
        {
            rotator = new Vector2(transform.rotation.x, 180f);
        }
        else                //если смотрим влево и двигаемся вправо - поворачиваемся
        {
            rotator = new Vector2(transform.rotation.x, 0f);
        }

        transform.rotation = Quaternion.Euler(rotator);
        isFacingRight = !isFacingRight;
    }
    #endregion

    //Прыжок
    #region Jump Function
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded())
        {
            StartCoroutine(JumpCoroutine());
        }
        if (context.performed && doubleJump && !isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
            doubleJump = false;
        }
    }
    //Первый прыжок
    private IEnumerator JumpCoroutine()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        doubleJump = true;

        yield return new WaitForSeconds(jumpDuration);

        doubleJump = false;
    }
    #endregion

    //Рывок
    #region Dash Function
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)   //если клавиша сработала и можем сделать рывок - рывок
        {
            StartCoroutine(DashCoroutine());
        }
    }
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        float dashDirection = isFacingRight ? 1 : -1;               //проверям направление персонажа

        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);              //задаем длительность рывка

        isDashing = false;
        
        if (isGliding)  //если персонаж находится в планировании - скорость падения переходит в скорость планирования            
        {
            rb.velocity = new Vector2(0f, -fallingSpeed);
            yield return new WaitForSeconds(dashCooldownInGlide);   //задаем перезарядку при планировании 
        }
        else            //иначе востанавливаем гравитацию и первоначальный метод движения
        {
            rb.gravityScale = initialGravityScale;
            rb.velocity = new Vector2(0f, rb.velocity.y);
            yield return new WaitForSeconds(dashCooldown);          //задаем перезарядку на земле и при прыжке
        }

        canDash = true;
    }
    #endregion

    //Высокий прыжок
    #region HighJump Function
    public void OnHighJump(InputAction.CallbackContext context)
    {
        if (glideStatus && context.started && isGrounded())   //если персонаж на земле, статус прыжка активирован и клавиша нажата
        {
            glideStatus = false;
            moveSpeed = 0f;
        }
        if (context.performed && isGrounded())                //если персонаж на земле и клавиша сработала
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * highJumpForce);
            moveSpeed = speedController;
        }
        else if (context.canceled)                                      //если клавиша отпущенна
        {
            glideStatus = true;
            moveSpeed = speedController;
        }
    }
    #endregion

    //Планирование
    #region Glide Function
    public void OnGlide(InputAction.CallbackContext context)
    {
        if (context.performed && rb.velocity.y < 0 && !isGrounded())  //если клавиша сработала и персонаж падает
        {
            isGliding = true;
            rb.gravityScale = 0;
            moveSpeed = glindingSpeed;
            rb.velocity = new Vector2(rb.velocity.x, -fallingSpeed);
        }
        else                                            //иначе выходим из планирования
        {
            isGliding = false;
            moveSpeed = speedController;
            rb.gravityScale = initialGravityScale;
        }
    }
    #endregion
    //Колайдер столкновения с землей
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);
    }
}