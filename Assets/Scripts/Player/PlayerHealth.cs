using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public int maxHealth = 10;
        public int health;
        public float pushForce = 45f;

        private Animator animator;
        private Rigidbody2D rb;

        private void Start()
        {
            health = maxHealth;
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        public void TakeDamage(int damage)
        {
            animator.SetTrigger("isDamageLow");
            health -= damage;

            if (health <= 0)
            {
                Debug.Log("PlayerDie");
                //Destroy(gameObject);
            }
        }
        public void TakeSoulDamage(int damage)
        {

        }
    }
}
