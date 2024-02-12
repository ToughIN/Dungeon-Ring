using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/CombatStance")]
public class AIState_CombatStance : AIState
{
    //1. 基于与角色的角度和距离为攻击状态选择一个攻击
    //2. 在这里处理攻击逻辑，当等待攻击时
    //3. 如果目标走出范围，切换到追逐模式
    //4. 如果目标消失，切换到静止模式
    [Header("Attacks")] 
    public List<AICharacterAttackAction> aiCharacterAttacks; //所有可能进行的攻击
    private List<AICharacterAttackAction> potentialAttacks; //在本状态中创建的链表，在这个阶段所有可能的攻击
    private AICharacterAttackAction choosenAttack;
    private AICharacterAttackAction previousAttack;
    protected bool hasAttack = false;

    [Header("Combo")] [SerializeField] protected bool canPerformCombo = false; //能否在第一次攻击后连击
    [SerializeField] protected int chanceToPerformCombo = 25; //进行连击的几率
    [SerializeField] private bool hasRolledForComboChance = false; //是否重新计算几率                                      

    [Header("Engagement Distance")] [SerializeField]
    public float maximumEngagementDistance = 5;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)//如果正在执行动作，返回
            return this;
        if (!aiCharacter.navmeshAgent.enabled)//如果navmeshAgent没有启用，返回
            aiCharacter.navmeshAgent.enabled = true;
        
        if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            //如果超出角度，转向
            if (aiCharacter.aiCharacterCombatManager.viewableAngle >
                aiCharacter.aiCharacterCombatManager.detectionFOV / 2)
            {
                aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);
            }
        }
        
        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);
        
        //如果目标不存在，切换到静止状态
        if(aiCharacter.aiCharacterCombatManager.currentTarget==null)
            return SwitchState(aiCharacter,aiCharacter.idleState);
        //如果没有攻击模式，选择一个攻击
        if (!hasAttack)
        {
            GetNewAttack(aiCharacter);
        }
        else
        {
            aiCharacter.attackState.currentAttack=choosenAttack;   
            //如果可以连击，检查连击几率
            return SwitchState(aiCharacter, aiCharacter.attackState);

        }
        if(aiCharacter.aiCharacterCombatManager.distanceFromTarget>maximumEngagementDistance)
            return SwitchState(aiCharacter,aiCharacter.pursueState);
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navmeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position,path);
        aiCharacter.navmeshAgent.SetPath(path);
        
        
        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAction>();
        
        foreach (var potentialAttack in aiCharacterAttacks)
        {
            
            if(potentialAttack.minimumAttackDistance>aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            if(potentialAttack.maximumAttackDistance<aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;
            if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;
            potentialAttacks.Add(potentialAttack);
        }
        if(potentialAttacks.Count<=0)
            return;
        var totalWeight = 0;
        foreach (var potentialAttack in potentialAttacks)
        {
            totalWeight += potentialAttack.attackWeight;
        }
        var randomWeight= Random.Range(1, totalWeight+1);
        var processedWeight = 0;
        foreach (var attack in potentialAttacks)
        {
            processedWeight += attack.attackWeight;

            if (randomWeight <= processedWeight)
            {
                choosenAttack = attack;
                previousAttack = choosenAttack;
                hasAttack = true;
            }
        }
        //4. 根据权重选择一个攻击
        //5. 选择一个攻击后将其送到攻击状态中
    }

    protected virtual bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;
        
        int randomPercentage= Random.Range(0, 100);

        if (randomPercentage < outcomeChance)
        {
            outcomeWillBePerformed = true;
        }

        return outcomeWillBePerformed;
    }
    
    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasAttack= false;
        hasRolledForComboChance = false;
        

    }
    


}