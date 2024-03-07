using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player {
    public class Animations : MonoBehaviour
    {
        [Header("Movement")]
        static public bool move;

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
            animator.SetBool("isMove", move);
        }
    }
}
