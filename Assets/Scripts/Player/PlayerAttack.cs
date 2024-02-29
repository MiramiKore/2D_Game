using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    public bool isAttack;
    public int attackDamage;
    public Transform AttackPoint;
    public float attackRange = 0.5f;
    [SerializeField] private float attackCooldown = 1f;
    public LayerMask enemyLayers;
    [SerializeField] private bool canAttack = true;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        isAttack = Animations.attack;

        if (context.performed && canAttack)
        {
            StartCoroutine(AttackCoroutine());
            animator.SetTrigger("isAttack");
        }
    }

    private IEnumerator AttackCoroutine()
    {
        canAttack = false;
        Attack();

        yield return new WaitForSecondsRealtime(attackCooldown);

        canAttack = true;
    }

    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
            return;

        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }
}
