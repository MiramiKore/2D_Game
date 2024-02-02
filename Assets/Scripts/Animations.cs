using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Animations : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private bool damage;
    [SerializeField] private bool attack;

    Animator animator;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isAttack", attack);
        animator.SetBool("isDamage", damage);



        if (UserInput.instance.controls.Attacking.Attack.WasPerformedThisFrame())
        {
            attack = true;
        }
        else
        {
            attack = false;
        }

        if (UserInput.instance.controls.Damaging.Damage.WasPerformedThisFrame())
        {
            damage = true;
        }
        else
        {
            damage= false;
        }
    }
}
