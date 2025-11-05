using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    //public PlayerController playerController;

    private const string PARAM_IS_MOVING = "IsMovingBool";
    private const string PARAM_IS_MOVING_BACK = "IsMovingBack";
    private const string PARAM_IS_MOVING_RIGHT = "IsMovingRight";
    private const string PARAM_IS_MOVING_LEFT = "IsMovingLeft";
    private const string PARAM_IS_JUMPING_UP = "IsJumpingUp";
    private const string PARAM_IS_JUMPING_DOWN = "IsJumpingDown";
    private const string PARAM_IS_ATTACKING = "IsAttacking";

    public PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            animator.SetBool(PARAM_IS_MOVING, true);
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            animator.SetBool(PARAM_IS_MOVING, false);
        }
        
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            animator.SetBool(PARAM_IS_MOVING, true);
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            animator.SetBool(PARAM_IS_MOVING, false);
        }

        if(Input.GetMouseButtonDown(0))
        {
            animator.SetBool(PARAM_IS_ATTACKING, true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool(PARAM_IS_ATTACKING, false);
        }
    }


    public enum PlayerAnimationStates
    {
        Walking,
        Jumping
    }
}

