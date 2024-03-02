using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage;
    public bool attack;
    public PlayerHealth playerHealth;
    [SerializeField] private float attackEndTime = 0.5f;
    [SerializeField] private float soulCoefficient = 9.6f;
    private int attackSoulCoefficient;                      //атака по коэффиценту души

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        attackSoulCoefficient = (int)(attackDamage * (soulCoefficient / 8));

        attackDamage = (int)(attackDamage + (attackDamage * soulCoefficient));
    }

    private void Update()
    {
        animator.SetBool("isGuardianAttack", attack);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(attackDamage);
            attack = true;
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackEndTime);

        attack = false;
    }
}
