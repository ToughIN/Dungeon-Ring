using System;
using ToufFrame;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [HideInInspector]public AICharacterNetworkManager aiCharacterNetworkManager;
    [HideInInspector]public AICharacterCombatManager aiCharacterCombatManager;
    [HideInInspector]public AICharacterLocomotionManager aiCharacterLocomotionManager;
    
    
    [Header("Navmesh Agent")] public NavMeshAgent navmeshAgent;    
    
    [Header("Current State")] [SerializeField]
    private AIState currentState;
    public AIState CurrentState => currentState;

    [Header("States")]
     public AIState_Idle idleState;
     public AIState_Pursue pursueState;
     public AIState_CombatStance combatStanceState;
     public AIState_Attack attackState;
     
    
    protected override void Awake()
    {
        base.Awake();
        aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
        aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        navmeshAgent = GetComponent<NavMeshAgent>();
        
        idleState= Instantiate(idleState);
        pursueState= Instantiate(pursueState);
        
        currentState = idleState;
        
    }
    
    protected override void Update()
    {
        base.Update();
        
        aiCharacterCombatManager.HandleActionRecovery(this);
        
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(IsOwner)
        {
            ProcessStateMachine();
        }
        
    }

    private void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this) ?? null;
        
        if (nextState != null && nextState != currentState)
        {
            currentState= nextState;
        }
        navmeshAgent.transform.localRotation = Quaternion.identity;
        navmeshAgent.transform.localPosition = Vector3.zero;

        if (aiCharacterCombatManager.currentTarget != null)//如果有目标，就设置目标方向和视角，AICharacterCombatManager
        {
            aiCharacterCombatManager.targetDirection =
                aiCharacterCombatManager.currentTarget.transform.position - transform.position;
            aiCharacterCombatManager.viewableAngle =
                WorldUtilityManager.Instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetDirection);
            aiCharacterCombatManager.distanceFromTarget=Vector3.Distance(transform.position,aiCharacterCombatManager.currentTarget.transform.position);
        }

        if (navmeshAgent.enabled)
        {
            Vector3 agentDestination = navmeshAgent.destination;
            float remainingDistance = navmeshAgent.remainingDistance;

            if (remainingDistance > navmeshAgent.stoppingDistance)
            {
                aiCharacterNetworkManager.isMoving.Value = true;
            }
            else
            {
                aiCharacterNetworkManager.isMoving.Value = false;
            }
        }
    }

}