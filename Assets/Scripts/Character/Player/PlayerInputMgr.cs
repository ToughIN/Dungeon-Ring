using ToufFrame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class PlayerInputMgr : MonoSingletonBase<PlayerInputMgr>
{
    public PlayerManager player;
    
    
    private PlayerControls _playerControls; 
    
    [Header("CAMERA MOVEMENT INPUT")]
    [SerializeField] private Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;
    
    [Header("PLAYER MOVEMENT INPUT")]
    [SerializeField] private Vector2 movementInput;
    public float verticalInput;
    public  float horizontalInput;
    public float moveAmount;

    [Header("PLAYER ACTION INPUT")] 
    [SerializeField]private bool dodgeInput = false;
    [SerializeField]private bool sprintInput = false;
    [SerializeField]private bool jumpInput= false;
    [SerializeField] private bool switch_Right_Weapon_Input = false;
    [SerializeField] private bool switch_Left_Weapon_Input = false;

    [Header("LOCK ON INPUT")] 
    [SerializeField] private bool lockOn_Input;
    [SerializeField] private bool lock_Right_Input;
    [SerializeField] private bool lock_Left_Input;
    private Coroutine lockOnCoroutine;

    
    [Header("Bumper Inputs")] 
    [SerializeField] private bool commonAttack = false; //RB || LMB, lightAttack or release magic
    [SerializeField] private bool heavyAttack = false; //RT || shift+LMB, HeavyAttack

    
    [Header("Trigger Inputs")] 
    [SerializeField] private bool hold_HeavyAttack_Input = false;
    // [SerializeField] private bool Hold_RB_Input = false;
    
   
    
    private void Start()
    {
        SceneManager.activeSceneChanged+=OnSceneChanged;

        Instance.enabled = false;
        if (_playerControls != null)
        {
            _playerControls.Disable();

        }
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex())
        {
            Instance.enabled = true;
            if (_playerControls != null)
            {
                _playerControls.Enable();
            }
        }
        else
        {
            Instance.enabled = false;
            if (_playerControls != null)
            {
                _playerControls.Disable();
            }
        }
    }

    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            
            //Some Inputs Can Share The Same Action
            //Common
            _playerControls.PlayerAction.Dodge.performed += i => dodgeInput = true;
            _playerControls.PlayerAction.Sprint.performed += i => sprintInput = true;// HOLDING THE INPUT, SET THE INPUT TO TRUE
            _playerControls.PlayerAction.Sprint.canceled += i => sprintInput = false;// RELEASING THE INPUT, SET THE INPUT TO FALSE
            _playerControls.PlayerAction.Jump.performed += i =>jumpInput = true;
            _playerControls.PlayerAction.LockOn.performed += i => lockOn_Input = true;
            _playerControls.PlayerAction.SwitchRightWeapon.performed += i => switch_Right_Weapon_Input = true;
            _playerControls.PlayerAction.SwitchLeftWeapon.performed += i => switch_Left_Weapon_Input = true;
            
            _playerControls.PlayerAction.CommonAttack.performed += i => commonAttack = true;//TODO: CHANGE THE NAME OF THE INPUT, ADD THE KEYBOARD INPUT
            
            _playerControls.PlayerAction.HeavyAttack.performed += i => heavyAttack = true;//TODO: CHANGE THE NAME OF THE INPUT, ADD THE KEYBOARD INPUT
            _playerControls.PlayerAction.HoldHeavyAttack.performed += i => hold_HeavyAttack_Input = true;//TODO: CHANGE THE NAME OF THE INPUT, ADD THE KEYBOARD INPUT
            _playerControls.PlayerAction.HoldHeavyAttack.canceled += i => hold_HeavyAttack_Input = false;//TODO: CHANGE THE NAME OF THE INPUT, ADD THE KEYBOARD INPUT
            
            
            //Some Can't
            //GamPad
            _playerControls.PlayerMovement.GamePadMovement.performed += i=> movementInput=i.ReadValue<Vector2>();
            _playerControls.PlayerCamera.GamepadMovement.performed += i=> cameraInput=i.ReadValue<Vector2>();
            _playerControls.PlayerAction.SeekRightLockOnTarget.performed += i => lock_Right_Input = true;
            _playerControls.PlayerAction.SeekLeftLockOnTarget.performed += i => lock_Left_Input = true;
            
            //Mouse & Keyboard
            _playerControls.PlayerMovement.KeyboardMovement.performed += i => movementInput = i.ReadValue<Vector2>();
            _playerControls.PlayerMovement.KeyboardMovement.canceled += i => movementInput = Vector2.zero;
            _playerControls.PlayerCamera.MouseMovement.performed += i => cameraInput = i.ReadValue<Vector2>();

        }
        _playerControls.Enable();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                _playerControls.Enable();
            }
            else
            {
                _playerControls.Disable();
            }
        }
    }

    private void Update()
    {
        if(player==null)return;
        HandleAllInputs();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged-=OnSceneChanged;
    }
    
    private void HandleAllInputs()
    {
        
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        
        HandleCameraMovementInput();
        HandlePlayerMovementInput();
        HandleDodgeInput();
        HandleSprinting();
        HandleJumpInput();
        
        HandleRBInput();
        HandleRTInput();
        HandleChargeRTInput();
        
        HandleSwitchLeftWeaponInput();
        HandleSwitchRightWeaponInput();
        
    }

    #region Movement

    private void HandlePlayerMovementInput()
    {
        verticalInput=movementInput.y;
        horizontalInput=movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1f;
        }
        if(player==null)
            return;
        
        if(!player.playerNetworkManager.isLockedOn.Value|| player.playerNetworkManager.isSprinting.Value)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0,moveAmount,player.playerNetworkManager.isSprinting.Value);
        }
        else
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput,verticalInput,player.playerNetworkManager.isSprinting.Value);
        }
    }
    
    private void HandleCameraMovementInput()
    {
        cameraVerticalInput=cameraInput.y;
        cameraHorizontalInput=cameraInput.x;
    }

    #endregion

    #region LockOn

    private void HandleLockOnInput()
    {
        if (player == null) return;
        if(player.playerNetworkManager==null)Debug.Log(" player.playerNetworkManager==null");
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null)return;

            if (player.playerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;
            }
            
            //ATTEMPT TO FIND NEW TARGET

            if (lockOnCoroutine != null)
            {
                StopCoroutine(lockOnCoroutine);
            }

            lockOnCoroutine = StartCoroutine(PlayerCamera.Instance.WaitThenFindNewTarget());

        }
        
        
        
        if (lockOn_Input&&player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;
            PlayerCamera.Instance.ClearLockOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;
            return;
        }

        if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;
            
            // if we are aiming using ranged weapons return
            PlayerCamera.Instance.HandleLocatingLockOnTargets();

            if (PlayerCamera.Instance.nearestLockedOnTarget != null)
            {
                player.playerCombatManager.SetTarget(PlayerCamera.Instance.nearestLockedOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
            
            return;
        }
        
    }

    private void HandleLockOnSwitchTargetInput()
    {
        if (lock_Left_Input)
        {
            lock_Left_Input = false;
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.Instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.Instance.leftLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.Instance.leftLockOnTarget);
                }
            }
        }

        if (lock_Right_Input)
        {
            lock_Right_Input = false;
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.Instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.Instance.rightLockedOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.Instance.rightLockedOnTarget);
                }
            }
            
        }
    }

    #endregion
    
    #region Action
    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;
            
            //TODO: RETURN(DO NOTHING) IF MENU OR UI WINDOW IS OPEN
            player.playerLocomotionManager.AttemptToPerformDodge();
            
        }
        
    }

    private void HandleSprinting()
    {
        if (sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }
    
    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            //IF WE HAVE A UI OPEN RETURN WITHOUT DOING ANYTHING
            
            //ATTEMPT TO PERFROM
            player.playerLocomotionManager.AttemptToPerformJump();
            
            
        }
    }

    private void HandleRBInput()
    {
        if (commonAttack)
        {
            commonAttack = false;
            //TODO: RETURN(DO NOTHING) IF MENU OR UI WINDOW IS OPEN
            
            player.playerNetworkManager.SetCharacterActionHand(true);
            
            //TODO: IF WE ARE TWO HANDING THE WEAPON,USE THE TWO HANDED ACTION
            
            // if(player.playerCombatManager==null){Debug.Log( "player.playerCombatManager==null");
            //     return;
            // }
            // if(player.playerInventoryManager==null){Debug.Log( "player.playerInventoryManager==null");
            //     return;
            // }
            // if(player.playerInventoryManager.currentRightHandWeapon==null){Debug.Log( "player.playerInventoryManager.currentRightHandWeapon==null");
            //     return;
            // }
            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.commonAttack,player.playerInventoryManager.currentRightHandWeapon);
            
        }
    }

    private void HandleRTInput()
    {
        if (heavyAttack)
        {
            heavyAttack = false;
 
            player.playerNetworkManager.SetCharacterActionHand(true);
            
            //TODO: IF WE ARE TWO HANDING THE WEAPON,USE THE TWO HANDED ACTION
            
            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.heavyAttack,player.playerInventoryManager.currentRightHandWeapon);
            
        }
    }

    private void HandleChargeRTInput()
    {
        if (player.isPerformingAction)
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerNetworkManager.isChargingAttack.Value = true;
            }
        }
    }

    private void HandleSwitchRightWeaponInput()
    {
        if (switch_Right_Weapon_Input)
        {
            switch_Right_Weapon_Input = false;
            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }
    
    private void HandleSwitchLeftWeaponInput()
    {
        if (switch_Left_Weapon_Input)
        {
            switch_Left_Weapon_Input = false;
            player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    #endregion
    
    
  
}