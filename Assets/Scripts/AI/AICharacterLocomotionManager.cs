using ToufFrame;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterLocomotionManager : CharacterLocomotionManager
{
    [SerializeField] private float moveSpeed = 3.5f;
    
    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navmeshAgent.transform.rotation;
        }
    }

    public void MoveTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.navmeshAgent.Move(aiCharacter.navmeshAgent.transform.forward * moveSpeed * Time.deltaTime);
        }
    }
    
 
    
        
}