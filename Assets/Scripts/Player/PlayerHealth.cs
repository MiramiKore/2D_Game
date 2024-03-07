using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        //public int maxHealth = 10;
        //public int health;

        [Header("Stats")]
        [SerializeField] private float maxHealth = 200f;                //����������� ��������
        [SerializeField] private float soulCoefficient = 39.2f;         //���������� ����
        [SerializeField] private float physicalDmgResistance = 0.35f;   //���������� �������������
        [SerializeField] private float soulDmgResistance = 0.44f;       //������������� ����

        Enemy.EnemyAttack enemyAttack;

        private float currentHealth;    //������� �������� ���������
        private float currentSoul;      //������� ����������� ����

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();

            currentHealth = maxHealth + (maxHealth * soulCoefficient);  //������� �������� ��������� = ����������� �������� + (����������� �������� * ���������� ����)
            currentSoul = soulCoefficient;                              //������� ����������� ���� = ����������� ����
        }

        public void TakeSoulDamage(int damage)
        {
            currentSoul -= (soulDmgResistance * damage);                //�� ������� ����������� ���� �������� ������������� ���� * �� ����
            currentHealth = maxHealth + (maxHealth * currentSoul);      //������� �������� ����� ������������� �������� * �� ������� ����������� ����
        }
        //���� �� ��������
        public void TakeDamage(int damage)
        {
            currentHealth -= (damage * physicalDmgResistance);

            animator.SetTrigger("isDamageLow");

            if (currentHealth <= 0)  //���� ������� �������� ������ ��� ����� ���� - ���� �������
            {
                Die();
            }
        }
        //������ �����
        void Die()
        {
            Debug.Log("PlayerDie");
            //Destroy(gameObject);
        }
    }
    //public void TakeDamage(int damage)
    //{
    //    animator.SetTrigger("isDamageLow");
    //    health -= damage;

    //    if (health <= 0)
    //    {
    //        Debug.Log("PlayerDie");
    //        //Destroy(gameObject);
    //    }
    //}
}
}
