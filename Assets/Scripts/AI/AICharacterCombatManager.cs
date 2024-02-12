using Unity.Mathematics;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    [Header("Action Recovery")] 
    public float actionRecoveryTimer = 0;
    
    
    [Header("Target Information")] 
    public float distanceFromTarget;
    public float viewableAngle;

    public Vector3 targetDirection;
    
    [Header("Detection")] [SerializeField] private float detectionRadius = 15;
    public float detectionFOV = 120;
    
    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 25;


    protected override void Awake()
    {
        base.Awake();
        lockedOnTransform = GetComponentInChildren<LockOnTransform>().transform;
    }
    
    public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if(currentTarget != null)
            return;
        Collider[] colliders= Physics.OverlapSphere(aiCharacter.transform.position,detectionRadius,WorldUtilityManager.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();
            
            if(targetCharacter==null)
                continue;
            if(targetCharacter==aiCharacter)
                continue;
            if(targetCharacter.isDead.Value)
                continue;
            
            //是否能攻击此目标
            if (!WorldUtilityManager.Instance.CanIDamageThisTarget(aiCharacter.characterGroup,targetCharacter.characterGroup))
                continue;
            
            Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
            if (Vector3.Angle(targetDirection, aiCharacter.transform.forward) > detectionFOV / 2)
                continue;

            //画出路径
            Debug.DrawLine(aiCharacter.transform.position, targetCharacter.transform.position, Color.green, 1.0f);
            // 检查是否有直视路径
            if (!Physics.Raycast(aiCharacter.transform.position, targetDirection.normalized, out RaycastHit hit, detectionRadius,WorldUtilityManager.Instance.GetEnvironmentLayers()) 
                || hit.transform != targetCharacter.transform)
                continue;

            //用drwLine显示直视路径
            
            // 找到目标
            targetDirection=targetCharacter.transform.position-transform.position;
            viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, targetDirection);
            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
            PivotTowardsTarget(targetCharacter);
            break;
            
            
        }


    }
    
    public void PivotTowardsTarget(CharacterManager character)
    {
        //播放一个pivot动画，根据目标的位置来决定旋转的方向
        //这里有两个解决方案，一个是用动画，一个是直接用transform.rotation
        //这里用transform.rotation，用插值来平滑旋转
        Vector3 targetDirection = character.transform.position - transform.position;
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
        
    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation=aiCharacter.navmeshAgent.transform.rotation;
        }
        
    }

    public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
    {
        if(currentTarget==null)return;
        if(!aiCharacter.aiCharacterLocomotionManager.canRotate)
            return;
        
        if(!aiCharacter.isPerformingAction)return;
        
        Vector3 direction = currentTarget.transform.position - aiCharacter.transform.position;
        direction.y = 0;
        direction.Normalize();

        if (direction == Vector3.zero)
            direction = aiCharacter.transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        
        aiCharacter.transform.rotation=Quaternion.Slerp(aiCharacter.transform.rotation,targetRotation,attackRotationSpeed*Time.deltaTime);
        //1. 检查是否能旋转
        //2. 朝向目标
    }
    
    public void HandleActionRecovery(CharacterManager aiCharacter)
    {
        if (actionRecoveryTimer > 0)
        {
            if (!aiCharacter.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
            
        }
        
    }
    
}