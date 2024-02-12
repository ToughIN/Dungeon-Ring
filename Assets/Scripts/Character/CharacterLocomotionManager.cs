using System;
using UnityEngine;
using ToufFrame;
public class CharacterLocomotionManager : MonoBehaviour
{
    [SerializeField]private CharacterManager character;

    [Header("Ground Check & Jumping")] 
    [SerializeField] protected float gravityForce = -10f;
    
    [SerializeField]private LayerMask groundLayer;
    
    [SerializeField] private float groundCheckSphereRadius = 0.3f;
    [SerializeField] private Vector3 groundCheckSphereOffset = new Vector3(0,0,0);
    
    [SerializeField] protected Vector3 yVelocity; //控制角色上下的力

    [SerializeField] protected float groundYVelocity = 0;// THE FORCE AT WHICH OUR CHARACTER IS STICKING TO THE GROUND WHILST THEY ARE GROUNDED

    [SerializeField] protected float fallStartYVelocity = -5; // THE FORCE AT WHICH OUR CHARACTER BEGINS TO FALL WHEN THEY BECOME UNGROUNDED (RISES AS THEY FALL LONGER)

    [SerializeField]protected bool fallingVelocityHasBeenSet = false;
    [SerializeField]protected TriggerVariable<float> inAirTimer = new TriggerVariable<float>(0);
    
    
    

    [Header("Flags")] 
    public bool isRolling = false;
    public bool canRotate=true;
    public bool canMove = true;
    public TriggerVariable<bool> isGrounded = new TriggerVariable<bool>(true);

    
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected void Start()
    {
        inAirTimer.OnValueChange += (f =>
        {
            character.animator.SetFloat(GameStrings.VARIABLE_ANIMATOR_InAirTimer, f);
        });
        isGrounded.OnValueChange+=(b =>
        {
            character.animator.applyRootMotion = false;
            character.animator.SetBool(GameStrings.VARIABLE_ANITAMOR_IsGrounded, b);
        });
    }

    protected virtual void Update()
    {
        HandleGroundCheck();
        HandleGravity();
    }

    
    protected virtual void HandleGravity()
    {
        
        if (character.characterLocomotionManager.isGrounded.Value)// 角色在地面上
        {
            
            inAirTimer.Value = 0f;
            fallingVelocityHasBeenSet = false;

            // 确保角色在地面上时不会因为重力而“下沉”
            if (yVelocity.y < 0)
            {
                yVelocity.y = groundYVelocity; // 这里可以设置为0或一个很小的正值,目前为0
            }
        }
        else// 角色在空中
        {
            
            if (!fallingVelocityHasBeenSet&&yVelocity.y < 0)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity; // 初始下降速度
            }

            inAirTimer.Value += Time.deltaTime;
            yVelocity.y += gravityForce * Time.deltaTime; // 应用重力效果
        }

        // 应用最终计算出的移动
        character.characterController.Move(yVelocity * Time.deltaTime);
    }
    


    protected virtual void HandleGroundCheck()
    {
        character.characterLocomotionManager.isGrounded.Value = Physics.CheckSphere( character.transform.position + groundCheckSphereOffset, groundCheckSphereRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    protected void OnDrawGizmosSelected()
    {
        //
        Gizmos.DrawSphere(character.transform.position+groundCheckSphereOffset,groundCheckSphereRadius);
    }

    public void EnableCanRotate()
    {
        canRotate = true;
    }
    
    public void DisableCanRotate()
    {
        canRotate = false;
    }
    
}