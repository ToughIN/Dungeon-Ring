using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Pursue")]
public class AIState_Pursue : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {

        if(aiCharacter.isPerformingAction)
            return this;
        if(aiCharacter.aiCharacterCombatManager.currentTarget==null)
            return SwitchState(aiCharacter,aiCharacter.idleState);
        if(!aiCharacter.navmeshAgent.enabled)
            aiCharacter.navmeshAgent.enabled = true;

        //如果目标在视角外，就转向目标
        if (Mathf.Abs(aiCharacter.aiCharacterCombatManager.viewableAngle) >
            aiCharacter.aiCharacterCombatManager.detectionFOV / 2)
        {
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }
        
        
        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <=
            aiCharacter.navmeshAgent.stoppingDistance)
        {
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);
        }
        
        
        // aiCharacter.navmeshAgent.SetDestination(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position);
        
        //这样写是为了避免navmeshAgent的bug
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navmeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position,path);
        aiCharacter.navmeshAgent.SetPath(path);
        
        
        return this;
    }
}