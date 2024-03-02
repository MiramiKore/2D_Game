using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;     //перезарядка атаки
    [SerializeField] private float attackRange = 0.5f;      //радиус атаки
    [SerializeField] private float attackDuration = 0.2f;   //задержка до нанесения урона

    [Header("Stats")]
    [SerializeField] private int attackDamage = 15;         //урон персонажа
    [SerializeField] private float soulCoefficient = 9.6f;  //коэффицент души
    private int attackSoulCoefficient;                      //атака по коэффиценту души

    public Transform AttackPoint;                           //компонент (точка атаки)
    public LayerMask enemyLayers;                           //слой врага
    private bool canAttack = true;                          //возможность атаки

    Animator animator;                                      

    private void Start()
    {
        animator = GetComponent<Animator>();

        attackSoulCoefficient = (int)(attackDamage * (soulCoefficient/8));

        attackDamage = (int)(attackDamage + (attackDamage * soulCoefficient));
    }
    //Вызов атаки
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            StartCoroutine(AttackCoroutine());
            animator.SetTrigger("isAttack");
        }
    }
    //Корутина атаки
    private IEnumerator AttackCoroutine()
    {
        canAttack = false;

        yield return new WaitForSeconds(attackDuration);

        Attack();

        yield return new WaitForSecondsRealtime(attackCooldown);

        canAttack = true;
    }
    //Атака
    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeSoulDamage(attackSoulCoefficient);
            Debug.Log(attackSoulCoefficient);
            enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
        }
    }
    //Отображение зоны атаки
    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
            return;

        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }
}
