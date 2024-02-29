using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Animations : MonoBehaviour
{
    [Header("Movement")]
    static public bool move;

    [Header("Damage")]
    static public bool damageLow;
    static public bool damageBig;

    [Header("Attack")]
    static public bool attackUp;
    static public bool attack;

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
        animator.SetBool("isAttackUp", attackUp);
        animator.SetBool("isMove", move);
    }
}
