using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Animations : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private bool damageLow;
    [SerializeField] private bool damageBig;

    [Header("Attack")]
    [SerializeField] private bool attackUp;
    [SerializeField] private bool attack;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        AttackAnimation();
    }

    private void AttackAnimation()
    {
        animator.SetBool("isAttack", attack);
        animator.SetBool("isAttackUp", attackUp);
    }
}
