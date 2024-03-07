using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField] private float attackRange = 0.5f;      //������ �����

        [Header("Stats")]
        [SerializeField] private int attackDamage = 15;         //���� ���������
        [SerializeField] private float soulCoefficient = 9.6f;  //���������� ����
        private int attackSoulCoefficient;                      //����� �� ����������� ����

        public Transform AttackPoint;                           //��������� (����� �����)
        public LayerMask enemyLayers;                           //���� �����
        private bool canAttack = true;                          //����������� �����

        Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();

            attackSoulCoefficient = (int)(attackDamage * (soulCoefficient / 8));

            attackDamage = (int)(attackDamage + (attackDamage * soulCoefficient));
        }
        //����� �����
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed && canAttack)
            {
                animator.SetTrigger("isAttack");
            }
        }
        //�����
        public void Attack()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy.EnemyHealth>().TakeSoulDamage(attackSoulCoefficient);
                Debug.Log(attackSoulCoefficient);
                enemy.GetComponent<Enemy.EnemyHealth>().TakeDamage(attackDamage);
            }
        }
        //����������� ���� �����
        void OnDrawGizmosSelected()
        {
            if (AttackPoint == null)
                return;

            Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
        }
    }
}
