using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Idle")]
public class AIState_Idle : AIState
{
    public override void OnEnter()
    {
        Debug.Log("AIState_Idle OnEnter");
    }

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        // Debug.Log("AIState_Idle Tick");
        if (aiCharacter.characterCombatManager.currentTarget != null)
        {
            return SwitchState(aiCharacter,aiCharacter.pursueState);
        }
        else
        {
            aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);
            
            return this;
        }
    }
}