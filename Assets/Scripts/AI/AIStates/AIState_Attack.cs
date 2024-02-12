using System.Net.Security;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Attack")]
public class AIState_Attack : AIState
{
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool wilLPerformCombo = false;

    [Header("State Flags")] 
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")] [SerializeField]
    protected bool pivotAfterAttack = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }
        
        aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhilstAttacking(aiCharacter);
        
        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0,0,false);
        
        //将移动值设置为0

        //执行连击
        if (wilLPerformCombo && !hasPerformedCombo)
        {
            if (currentAttack.comboAction != null)
            {
                //如果可以连击
                // hasPerformedCombo = true;
                // currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
            }
        }

        if (aiCharacter.isPerformingAction)
            return this;
        
        if (!hasPerformedAttack)
        {
            // 如果目前仍然处于从一个动作中回复过来的状态，等待知道执行另一个
            if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
            {
                return this;
            }

            if (aiCharacter.isPerformingAction)
            {
                return this;
            }

            PerformAttack(aiCharacter);
            return this;

        }

        if (pivotAfterAttack)
        {
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }
        return SwitchState(aiCharacter, aiCharacter.combatStanceState);
    }

    protected void PerformAttack(AICharacterManager aiCharacter)
    {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);
        aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        
        hasPerformedAttack = false;
        hasPerformedCombo= false;
    }
    
    
}