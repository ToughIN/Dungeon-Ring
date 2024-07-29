using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    private PlayerManager player;
    
    [HideInInspector]public float verticalMovement;
    [HideInInspector]public float horizontalMovement;
    [HideInInspector]public float moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] private float walkingSpeed=2;
    [SerializeField] private float runningSpeed=5;
    [SerializeField] private float sprintingSpeed = 7;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] private int sprintingStaminaCost = 0;

    [Header("Jump")] 
    [SerializeField] private float jumpStaminaCost = 10;
    [SerializeField] private float jumpHeight = 4;
    [SerializeField] private float jumpForwardSpeed = 5;
    [SerializeField] private float freeFallSpeed = 2;
    private Vector3 jumpDirection;
    
    
    
    [Header("Dodge")] 
    private Vector3 rollDirection;

    [SerializeField] private float dodgeStaminaCost = 25;

    public GameObject testJumpObject;
    
    
    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();


    }

    protected override void Update()
    {
        base.Update();

        if (player.IsOwner)
        {
            player.characterNetWorkManager.verticalMovement.Value = verticalMovement;
            player.characterNetWorkManager.horizontalMovement.Value = horizontalMovement;
            player.characterNetWorkManager.moveAmount.Value = moveAmount;
        }
        else
        {
            moveAmount = player.characterNetWorkManager.moveAmount.Value;
            verticalMovement = player.characterNetWorkManager.verticalMovement.Value;
            horizontalMovement = player.characterNetWorkManager.horizontalMovement.Value;
            
            
            if(!player.playerNetworkManager.isLockedOn.Value|| player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0,moveAmount,player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement,verticalMovement,player.playerNetworkManager.isSprinting.Value);
            }
            
            
        }
        
    }


    public void HandleAllMovement()
    {
        HandleGroundMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }
    

    private void GetMovementValues()
    {
        verticalMovement = PlayerInputMgr.Instance.verticalInput;
        horizontalMovement = PlayerInputMgr.Instance.horizontalInput;
        moveAmount = PlayerInputMgr.Instance.moveAmount;

    }
    
    private void HandleGroundMovement()
    {
        if(!player.playerLocomotionManager.canMove)
            return;

        
        GetMovementValues();
        
        moveDirection=PlayerCamera.Instance.transform.forward*verticalMovement;
        moveDirection=moveDirection+PlayerCamera.Instance.transform.right*horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.characterController.Move(moveDirection*sprintingSpeed*Time.deltaTime);

        }
        else
        {
            if (PlayerInputMgr.Instance.moveAmount > 0.5f)
            {
                //move at a running speed
                player.characterController.Move(moveDirection*runningSpeed*Time.deltaTime);
           
            }
            else if (PlayerInputMgr.Instance.moveAmount <= 0.5f)
            {
                //move at a walking speed
                player.characterController.Move(moveDirection*walkingSpeed*Time.deltaTime);
            }    
        }
        

    }

    private void HandleJumpingMovement()
    {
        if (player.playerNetworkManager.isJumping.Value)
        {
            // player.playerNetworkManager.isJumping.Value = false;
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void HandleFreeFallMovement()
    {
        if (!player.playerLocomotionManager.isGrounded.Value)
        {
            Vector3 freeFallDirection;

            freeFallDirection = PlayerCamera.Instance.transform.forward *
                                PlayerInputMgr.Instance.verticalInput;
            freeFallDirection=freeFallDirection+PlayerCamera.Instance.transform.right*
                               PlayerInputMgr.Instance.horizontalInput;
            freeFallDirection.y = 0;
            
            player.characterController.Move(freeFallDirection*freeFallSpeed*Time.deltaTime);

        }
    }
    
    private void HandleRotation()
    {
        if(player.isDead.Value)
            return;
        if(!player.playerLocomotionManager.canRotate) 
            return;
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerNetworkManager.isSprinting.Value|| player.playerLocomotionManager.isRolling)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = PlayerCamera.Instance.cameraObject.transform.forward * verticalMovement;
                targetDirection += PlayerCamera.Instance.cameraObject.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
            else
            {
                if (player.playerCombatManager.currentTarget == null) return;

                Vector3 targetDirection;

                targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
        }
        else
        {
            targetRotationDirection=Vector3.zero;
            targetRotationDirection = PlayerCamera.Instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection=targetRotationDirection+PlayerCamera.Instance.cameraObject.transform.right*horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotaion=Quaternion.Slerp(transform.rotation,newRotation,Time.deltaTime*rotationSpeed);
            transform.rotation = targetRotaion;
        }

    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        // if we are moving, sprinting is true.
        if (moveAmount >= 0.5)
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }
        // if we are stationary/moving slowly, sprinting if false;
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.playerNetworkManager.currentStamina.Value-=sprintingStaminaCost*Time.deltaTime;
        }
        
        
    }

    public void AttemptToPerformDodge()
    {
        if (player.isPerformingAction)
            return;
        
        if(player.playerNetworkManager.currentStamina.Value<=0)
            return;
        
        //IF WE ARE MOVING WHEN WE ATTEMPT TO DODGE, WE PERFORM A ROLL
        if (PlayerInputMgr.Instance.moveAmount>0)
        {
            rollDirection = PlayerCamera.Instance.cameraObject.transform.forward *
                            PlayerInputMgr.Instance.verticalInput;
            rollDirection += PlayerCamera.Instance.cameraObject.transform.right *
                             PlayerInputMgr.Instance.horizontalInput;
            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;
            
            player.playerAnimatorManager.PlayTargetActionAnimation(GameStrings.ANIMATION_ROLL_FORWARD_01,true,true);
            player.playerLocomotionManager.isRolling = true;

        }
        // IF WE ARE STATIONARY, WE PERFORM A BACKSTEP
        else
        {
            player.playerAnimatorManager.PlayTargetActionAnimation(GameStrings.ANIMATION_ROLL_FORWARD_01,true,true);
        }

        player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;

    }

    public void AttemptToPerformJump()
    {
        // IF WE ARE PERFORMING A GENERAL, WE DO NOT WANT TO ALLOW A JUMP (TODO: CHANGE WHEN COMBAT IS ADDED)
        if (player.isPerformingAction)
        {
            // Debug.Log("isPerformingAction");
            return;
        }

        // IF WE AREA OUT OF STAMINA, WE DO NOT WISH TO ALLOW A JUMP
        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            // Debug.Log("currentStamina.Value <= 0");
            return;
        }
            
        
        // IF WE ARE ALREADY IN A JUMP, WE DO NOT WISH TO ALLOW A JUMP
        if (player.playerNetworkManager.isJumping.Value)
        {
            // Debug.Log("isJumping.Value");
            return;
        }

        // IF WE ARE NOT GROUNDED, WE DO NOT WISH TO ALLOW A JUMP
        if (!player.playerLocomotionManager.isGrounded.Value)
        {
            // Debug.Log("!isGrounded.Value");
            return;
        }
        
        // 双手武器单手武器跳跃动画不同 (TODO:)
        player.playerAnimatorManager.PlayTargetActionAnimation(GameStrings.ANIMATION_MAIN_JUMP_START_01,false);

        player.playerNetworkManager.isJumping.Value = true;
        
        //
        player.playerNetworkManager.currentStamina.Value -=jumpStaminaCost;


        jumpDirection = PlayerCamera.Instance.cameraObject.transform.forward *
                        PlayerInputMgr.Instance.verticalInput;
        jumpDirection += PlayerCamera.Instance.cameraObject.transform.right *
                         PlayerInputMgr.Instance.horizontalInput;
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            if (player.playerNetworkManager.isSprinting.Value)
            {
                jumpDirection *= 1;
            }
            else if (PlayerInputMgr.Instance.moveAmount > 0.5)
            {
                jumpDirection *= 0.5f;
            }
            else if (PlayerInputMgr.Instance.moveAmount <= 0.5)
            {
                jumpDirection *= 0.25f;
            }
        }
    }

    // 被动画 Main_Jump_Start_01 调用
    public void ApplyJumpVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
        
    }

}