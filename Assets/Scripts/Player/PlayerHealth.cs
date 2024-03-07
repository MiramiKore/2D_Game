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
        [SerializeField] private float maxHealth = 200f;                //максиальное здоровье
        [SerializeField] private float soulCoefficient = 39.2f;         //коэффицент души
        [SerializeField] private float physicalDmgResistance = 0.35f;   //физическое сопротивление
        [SerializeField] private float soulDmgResistance = 0.44f;       //сопротивление души

        Enemy.EnemyAttack enemyAttack;

        private float currentHealth;    //текущие здоровье персонажа
        private float currentSoul;      //текущая целостность души

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();

            currentHealth = maxHealth + (maxHealth * soulCoefficient);  //текущие здоровье персонажа = максиальное здоровье + (максиальное здоровье * коэффицент души)
            currentSoul = soulCoefficient;                              //текущая целостность души = коэффиценту души
        }

        public void TakeSoulDamage(int damage)
        {
            currentSoul -= (soulDmgResistance * damage);                //из текущей целостности души вычитаем сопротивление души * на урон
            currentHealth = maxHealth + (maxHealth * currentSoul);      //текущие здоровье равно максимальному здоровью * на текущую целостность души
        }
        //Урон по здоровью
        public void TakeDamage(int damage)
        {
            currentHealth -= (damage * physicalDmgResistance);

            animator.SetTrigger("isDamageLow");

            if (currentHealth <= 0)  //если текущие здоровье меньше или равно нулю - юнит умирает
            {
                Die();
            }
        }
        //Смерть юнита
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
