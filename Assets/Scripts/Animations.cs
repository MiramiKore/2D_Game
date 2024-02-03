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
        DamageAnimation();
    }

    private void AttackAnimation()
    {
        animator.SetBool("isAttack", attack);
        animator.SetBool("isAttackUp", attackUp);

        if (UserInput.instance.controls.Player.Attack.WasPerformedThisFrame())
        {
            attack = true;
        }
        else
        {
            attack = false;
        }

        if (UserInput.instance.controls.Player.VectorUp.IsPressed() && attack)
        {
            attackUp = true;
        }
        else
        {
            attackUp = false;
        }
    }

    private void DamageAnimation()
    {
        animator.SetBool("isDamageLow", damageLow);
        animator.SetBool("isDamageBig", damageBig);

        if (UserInput.instance.controls.Player.DamageLow.WasPerformedThisFrame())
        {
            damageLow = true;
        }
        else
        {
            damageLow = false;
        }

        if (UserInput.instance.controls.Player.DamageBig.WasPerformedThisFrame())
        {
            damageBig = true;
        }
        else
        {
            damageBig = false;
        }
    }
}
