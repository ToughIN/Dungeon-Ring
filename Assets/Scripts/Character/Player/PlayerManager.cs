using System.Collections;
using ToufFrame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{
    [Header("DEBUG MENU")] 
    [SerializeField]private bool respawnCharacter = false;

    [SerializeField] private bool switchRightWeapon = false;
    
    public UI_Canvas_PlayerHUD PlayerHUDCanvas 
    {get=>UIMgr.Instance.canvasDIC[GameStrings.ADDRESSABLES_UI_CANVAS_PlayerHUD] as UI_Canvas_PlayerHUD;}
    
    
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;

    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    
    [HideInInspector] public PlayerStatsManager playerStatsManager;

    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;

    [HideInInspector] public PlayerCombatManager playerCombatManager;

    
    protected override void Awake()
    {
        base.Awake();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();    
        playerAnimatorManager=GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager= GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
    }
    protected override void Update()
    {
        base.Update();

        // IF WE DO NOT OWN THIS GAMEOBJECT, THEN WE DO NOT HAVE CONTROL OVER IT
        if (!IsOwner)
            return;
        
        
        playerLocomotionManager.HandleAllMovement();
        // REGEN STAMINA
        playerStatsManager.RegenerateStamina();
        DebugMenu();
    }

    protected override void LateUpdate()
    {
        if(!IsOwner)
            return;
        base.LateUpdate();
        PlayerCamera.Instance.HandleAllCamaeraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        
        if (IsOwner)
        {
            PlayerCamera.Instance.player = this;
            PlayerInputMgr.Instance.player= this; 
            WorldSaveGameManager.Instance.player= this;
            
            //UPDATE THE TOTAL AMOUNT OF HEALTH OR STAMINA WHEN THE STAT LINKED TO EITHER CHANGES
            playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;
            
            //LOCK ON
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged +=
                playerNetworkManager.OnLockOnTargetIDChange; 
           
            
            // UPDATE UI STAT BARS WHEN A STAT CHANGES (HEALTH OR STAMINA)
            playerNetworkManager.currentHealth.OnValueChanged+= PlayerHUDCanvas.uiPanelHudBasic.SetNewHealthValue;
            // playerNetworkManager.currentHealth.OnValueChanged+= playerStatsManager.ResetHealthRegenTimer;
            playerNetworkManager.currentStamina.OnValueChanged+= PlayerHUDCanvas.uiPanelHudBasic.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged+= playerStatsManager.ResetStaminaRegenTimer;
        }

        //STATS
        playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;
        
        //EQUIPMENT
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsedID.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

        //FLAGS
        playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;
        
        // UPON CONNECTING, IF WE ARE THE OWNER OF THIS CHARACTER, BUT WE ARE NOT THE SERVER, RELOAD OUR CHARACTER DATA TO THIS NEWLY INSTANTIATED CHARACTER
        // WE DONT RUN THIS IF WE ARE THE SERVER, BECAUSE SINCE THEY ARE THE HOST, THEY ARE ALREADY LOADED IN AND DON'T NEED TO RELOAD
        if (IsOwner && !IsServer)
        {
            LoadGameFromCurrentCharacterData(ref WorldSaveGameManager.Instance.currentCharacterData);
        }

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
                Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        
        if (IsOwner)
        {
            //UPDATE THE TOTAL AMOUNT OF HEALTH OR STAMINA WHEN THE STAT LINKED TO EITHER CHANGES
            playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;
            
            //LOCK ON
            playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -=
                playerNetworkManager.OnLockOnTargetIDChange; 
           
            // UPDATE UI STAT BARS WHEN A STAT CHANGES (HEALTH OR STAMINA)
            playerNetworkManager.currentHealth.OnValueChanged-= PlayerHUDCanvas.uiPanelHudBasic.SetNewHealthValue;
            // playerNetworkManager.currentHealth.OnValueChanged+= playerStatsManager.ResetHealthRegenTimer;
            playerNetworkManager.currentStamina.OnValueChanged-= PlayerHUDCanvas.uiPanelHudBasic.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged-= playerStatsManager.ResetStaminaRegenTimer;
        }

        //STATS
        playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;
        
        //EQUIPMENT
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsedID.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

        //FLAGS
        playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;
    }


    private void OnClientConnectedCallback(ulong clientID)
    {
        WorldGameSessionManager.Instance.AddPlayerToActivePlayerList(this);
        
        // IF WE ARE THE SERVER, WE ARE THE HOST, SO WE NEED TO LOAD PLAYERS
        // YOU ONLY NEED TO LOAD OTHER PLAYERS GEAR TO SYNC IT IF YOU JOIN A GAME THATS ALREADY BEEN ACTIVE WITHOUT YOU BEING PRESENT
        if (!IsServer && IsOwner)
        {
            foreach (var player in WorldGameSessionManager.Instance.players)
            {
                if (player != this)
                {
                    player.LoadOtherPlayerCharacterWhenJoiningServer();
                }
                
            }
        }
        
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation=false)
    {
        if (IsOwner)
        {
            PlayerHUDCanvas.playerUIPopupManager.SendYouDiedPopUp();
        }
        
        
        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentCharacterData.charqacterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
        currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;
        
        currentCharacterData.vitality = playerNetworkManager.vitality.Value;
        currentCharacterData.endurance = playerNetworkManager.endurance.Value;

        //Inventory
        currentCharacterData.items_SO_Addressable_PathList = playerInventoryManager.GetItemsPahtList();
    }
    

    public void LoadGameFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        playerNetworkManager.characterName.Value = currentCharacterData.charqacterName;
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition,currentCharacterData.yPosition,currentCharacterData.zPosition);
        transform.position = myPosition;
        
        playerNetworkManager.vitality.Value = currentCharacterData.vitality;
        playerNetworkManager.endurance.Value = currentCharacterData.endurance;
        
        //Set Health
        playerNetworkManager.maxHealth.Value = currentCharacterData.currentHealth;
        playerNetworkManager.currentHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
        PlayerHUDCanvas.uiPanelHudBasic.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
        
        //SET STAMINA
        playerNetworkManager.maxStamina.Value = currentCharacterData.currentStamina;
        playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
        PlayerHUDCanvas.uiPanelHudBasic.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);

        //Inventory
        playerInventoryManager.LoadItemsFromPathList(currentCharacterData.items_SO_Addressable_PathList);
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();
        if (IsOwner)
        {
            isDead.Value = false;
            playerNetworkManager.currentHealth.Value= playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currentStamina.Value= playerNetworkManager.maxStamina.Value;
            //restore focus points
            
            //play rebirth effects
            playerAnimatorManager.PlayTargetActionAnimation("Empty",false);
        }
        
    }




    private void LoadOtherPlayerCharacterWhenJoiningServer()
    {
        //SYNC WEAPONS
        playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
        playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);

        //ARMOR


        if (playerNetworkManager.isLockedOn.Value)
        {
            playerNetworkManager.OnLockOnTargetIDChange(0,playerNetworkManager.currentTargetNetworkObjectID.Value);
        }
}

    private void DebugMenu()
    {
        if (respawnCharacter)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }

        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            playerEquipmentManager.SwitchRightWeapon();
        }
    }

}