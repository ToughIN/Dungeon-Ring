using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

public class CharacterAnimatorManager : MonoBehaviour
{
    private CharacterManager character;

    private int vertical;
    private int horizontal;
    
    [Header("Flags")]
    public bool applyRootMotion = false;


    [Header("Damage Animations")] 
    public string lastDamageAnimationPlayed;
    
    
    public List<string> forward_Medium_Damage = new List<string>();
    public List<string> bakcward_Medium_Damage = new List<string>();
    public List<string> left_Medium_Damage = new List<string>();
    public List<string> right_Medium_Damage = new List<string>();

    
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash(GameStrings.VARIABLE_ANIMATOR_Vertical);
        horizontal = Animator.StringToHash(GameStrings.VARIABLE_ANIMATOR_Horizontal);
    }

    protected virtual void Start()
    {
        forward_Medium_Damage.Add(GameStrings.ANIMATION_HIT_FORWARD_MEDIUM_01);
        forward_Medium_Damage.Add(GameStrings.ANIMATION_HIT_FORWARD_MEDIUM_02);
        
        bakcward_Medium_Damage.Add(GameStrings.ANIMATION_HIT_BACKWARD_MEDIUM_01);
        bakcward_Medium_Damage.Add(GameStrings.ANIMATION_HIT_BACKWARD_MEDIUM_02);
        
        left_Medium_Damage.Add(GameStrings.ANIMATION_HIT_LEFT_MEDIUM_01);
        left_Medium_Damage.Add(GameStrings.ANIMATION_HIT_LEFT_MEDIUM_02);
        
        right_Medium_Damage.Add(GameStrings.ANIMATION_HIT_RIGHT_MEDIUM_01);
        right_Medium_Damage.Add(GameStrings.ANIMATION_HIT_RIGHT_MEDIUM_02);
    }

    public string GetRandomAnimationFromList(List<string> animationList)
    {
        List<string> finalList = new List<string>();

        foreach (var item in animationList)
        {
            finalList.Add(item);
        }
        
        //CHECK IF WE HAVE ALREADY PLAYED THIS DAMAGE ANIMATION SO IT DOESNT REPEAT
        finalList.Remove(lastDamageAnimationPlayed);

        // CHECK THE LIST FOR NULL ENTRISES, AND REMOVE THEM
        for (int i = finalList.Count; i > -1; i--)
        {
            if (finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }

        int randomValue = UnityEngine.Random.Range(0, finalList.Count);

        return finalList[randomValue];
    }
    
    public void UpdateAnimatorMovementParameters(float horizontalMovement,float verticalMovement,bool isSprinting)
    {
        float snappedHorizontal=AdjustValue(horizontalMovement) ;
        float snappedVertical=AdjustValue(verticalMovement) ;
        if (isSprinting)
        {
            snappedVertical = 2;
        }
        character.animator.SetFloat(horizontal,snappedHorizontal,0.1f,Time.deltaTime );
        character.animator.SetFloat(vertical,snappedVertical,0.1f,Time.deltaTime );
    }
    
    /// <summary>
    /// 控制移动动画参数，使其与移动契合
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    float AdjustValue(float input)
    {
        // 如果输入值为0，则直接返回0（避免负零的问题）
        if (input == 0)
        {
            return 0;
        }
        // 获取绝对值并根据条件返回相应的值
        float absInput = Mathf.Abs(input);
        if (absInput < 0.5f)
        {
            return 0.5f * Mathf.Sign(input); // 保持原始数值的符号
        }
        else
        {
            return 1.0f * Mathf.Sign(input); // 保持原始数值的符号
        }
    }

 
    public virtual void PlayTargetActionAnimation(
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion=true,
        bool canRotate=false,
        bool canMove=false)
    {
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation,0.2f);
        // CAN BE USED TO STOP CHARACTER FROM ATTEMPTING NEW ACTIONS
        // FOR EXAMPLE, IF YOU GET DAMAGED, AND BEGIN PERFORMING A DAMAGE ANIMATION
        // THI S FLAG WILL TURN TRUE IF YOU ARE STUNNED
        // WE CAN THEN CHECK FOR THIS BEFORE ATTEMPTING NEW ACTIONS
        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;
        
        // TELL THE SERVER/HOST WE PLAYED AN ANIMATION
        character.characterNetWorkManager.NotifyTheServerOfActionAnimationServerRPC(NetworkManager.Singleton.LocalClientId,targetAnimation,applyRootMotion);
        
    }
    
    public virtual void PlayTargetAttackActionAnimation(EAttackType attackType,
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion=true,
        bool canRotate=false,
        bool canMove=false)
    {
        //KEEP TRACK OF LAST ATTACK PERFORMED(FOR COMBOS
        // KEEP TRACK OF CURRENT ATTACK TYPE(LIGHT HEAVY ETC)
        // UPDATE ANIMATION SET TO CURRENT WEAPONS ANIMATIONS
        //DECIDE IF OUR ATTACK CAN BE PARRIED
        //TELL THE NETWORK OUR "ISATTACKING" FLAG IS ACTIVE(FOR COUNTER DAMAGE ETC
        
        character.characterCombatManager.currentAttackType = attackType;
        character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation,0.2f);
        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;
        
        // TELL THE SERVER/HOST WE PLAYED AN ANIMATION
        character.characterNetWorkManager.NotifyTheServerOfAttackActionAnimationServerRPC(NetworkManager.Singleton.LocalClientId,targetAnimation,applyRootMotion);
        
    }
    
    public virtual void EnableCanDoCombo()
    {}
    
    public virtual void DisableCanDoCombo()
    {}
    
    
}