using ToufFrame;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterLocomotionManager : CharacterLocomotionManager
{
    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navmeshAgent.transform.rotation;
        }
    }
    
        
}