using UnityEngine;
using UnityEngine.AI;

public class AIState : ScriptableObject
{
    public virtual void OnEnter()
    {
        
    }
    
    public virtual void OnExit()
    {
        
    }
    
    public virtual AIState Tick(AICharacterManager aiCharacter)
    {
        Debug.Log("AIState Tick");
        return this;
    }
    
    protected virtual AIState SwitchState(AICharacterManager aiCharacter,AIState newState)
    {
        ResetStateFlags(aiCharacter);
        aiCharacter.CurrentState.OnExit();
        newState.OnEnter();
        return newState;
    }

    protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
    {
            
    }
    
    
        
}