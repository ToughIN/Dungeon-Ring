using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;
    
    public WeaponModelInstantiationSlot rightHandSlot;
    public WeaponModelInstantiationSlot leftHandSlot;

    [SerializeField] private WeaponManager rightWeaponManager;
    [SerializeField] private WeaponManager leftWeaponManager;
    
    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;
    
    protected override void Awake()
    {
        base.Awake();
        
        player = GetComponent<PlayerManager>();
        
        InitializeWeaponSlots();
    }

    protected override void Start()
    {
        base.Start();
        LoadWeaponOnBothHands();
    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots= GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == EWeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if(weaponSlot.weaponSlot == EWeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    #region Right Weapon

    public void LoadRightWeapon()
    {
        if (player.playerInventoryManager.currentRightHandWeapon != null)
        {
            //REMOVE THE OLD WEAPON
            rightHandSlot.UnloadWeapon();
            
            //BRING IN THE NEW WEAPON
            rightHandWeaponModel=Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            rightWeaponManager=rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(player,player.playerInventoryManager.currentRightHandWeapon);
        }
            
    }

    public void SwitchRightWeapon()
    {
        if(!player.IsOwner)
            return;
        player.playerAnimatorManager.PlayTargetActionAnimation(GameStrings.ANIMATION_Swap_Right_Weapon_01,false,false,true,true);

        SO_WeaponSoItem selectSoWeaponSo = null;

        player.playerInventoryManager.rightHandWeaponIndex += 1;

        // 若切换武器超出范围
        if (player.playerInventoryManager.rightHandWeaponIndex < 0 ||
            player.playerInventoryManager.rightHandWeaponIndex > 2)
        {
            player.playerInventoryManager.rightHandWeaponIndex = 0;
            
            float weaponCount = 0;
            SO_WeaponSoItem firstSoWeaponSo = null;
            int firstWeaponPosition = 0;
            
            for(int i=0;i<player.playerInventoryManager.weaponsInRightHandSlots.Length;i++)
            {
                if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID !=
                    WorldItemDataBase.Instance.unarmedSoWeaponSo.itemID)
                {
                    weaponCount++;
                    
                    //获取第一个带有武器的槽位
                    if (firstSoWeaponSo == null)
                    {
                        firstSoWeaponSo = player.playerInventoryManager.weaponsInRightHandSlots[i];
                        firstWeaponPosition = i;
                    }
                }
            }

            // 只有一把武器或没有武器，切成空手
            if (weaponCount <= 1)
            {
                player.playerInventoryManager.rightHandWeaponIndex = -1;
                selectSoWeaponSo = WorldItemDataBase.Instance.unarmedSoWeaponSo;
                player.playerNetworkManager.currentRightHandWeaponID.Value = selectSoWeaponSo.itemID;
            }
            else
            {
                player.playerInventoryManager.rightHandWeaponIndex=firstWeaponPosition;
                player.playerNetworkManager.currentRightHandWeaponID.Value = firstSoWeaponSo.itemID;
            }

            return;
        }

        foreach (SO_WeaponSoItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
        {
            if (player.playerInventoryManager
                    .weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID !=
                WorldItemDataBase.Instance.unarmedSoWeaponSo.itemID)
            {
                selectSoWeaponSo=player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                //ASSIGN THE NETWORK WEAPON ID SO IT SWITCHES FOR ALL CONNECTED PLAYERS
                player.playerNetworkManager.currentRightHandWeaponID.Value =
                    player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
                return;
            }
        }
        
        if (selectSoWeaponSo == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
        {
            SwitchRightWeapon();
        }
        
    }
    


    #endregion


    #region Left Weapon

    public void LoadLeftWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            //REMOVE THE OLD WEAPON
            leftHandSlot.UnloadWeapon();
            
            //BRING IN THE NEW WEAPON
            leftHandWeaponModel=Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel);
            leftWeaponManager=leftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(player,player.playerInventoryManager.currentLeftHandWeapon);
        }
    }

    public void SwitchLeftWeapon()
    {
        if (!player.IsOwner)
            return;
        player.playerAnimatorManager.PlayTargetActionAnimation(GameStrings.ANIMATION_Swap_Left_Weapon_01, false, false,
            true, true);

        SO_WeaponSoItem selectSoWeaponSo = null;

        player.playerInventoryManager.leftHandWeaponIndex += 1;

        // 若切换武器超出范围
        if (player.playerInventoryManager.leftHandWeaponIndex < 0 ||
            player.playerInventoryManager.leftHandWeaponIndex > 2)
        {
            player.playerInventoryManager.leftHandWeaponIndex = 0;

            float weaponCount = 0;
            SO_WeaponSoItem firstSoWeaponSo = null;
            int firstWeaponPosition = 0;

            for (int i = 0; i < player.playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
            {
                if (player.playerInventoryManager.weaponsInLeftHandSlots[i].itemID !=
                    WorldItemDataBase.Instance.unarmedSoWeaponSo.itemID)
                {
                    weaponCount++;

                    //获取第一个带有武器的槽位
                    if (firstSoWeaponSo == null)
                    {
                        firstSoWeaponSo = player.playerInventoryManager.weaponsInLeftHandSlots[i];
                        firstWeaponPosition = i;
                    }
                }
            }

            // 只有一把武器或没有武器，切成空手
            if (weaponCount <= 1)
            {
                player.playerInventoryManager.leftHandWeaponIndex = -1;
                selectSoWeaponSo = WorldItemDataBase.Instance.unarmedSoWeaponSo;
                player.playerNetworkManager.currentLeftHandWeaponID.Value = selectSoWeaponSo.itemID;
            }
            else
            {
                player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosition;

            }
        }
    }



    #endregion
    
    //DAMAGE COLLIDER
    public void OpenDamageCollider()
    {
        // OPEN RIGHT WEAPON DAMAGE COLLIDER
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            rightWeaponManager.meleeDamageCollider.EnableDamageCollider();
        }
        // OPEN LEFT WEAPON DAMAGE COLLIDER
        else if(player.playerNetworkManager.isUsingLeftHand.Value)
        {
            leftWeaponManager.meleeDamageCollider.EnableDamageCollider();
        }
        // PLAY WHOOSH SFX
        
    }
    
    public void CloseDamageCollider()
    {
        // OPEN RIGHT WEAPON DAMAGE COLLIDER
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }
        // OPEN LEFT WEAPON DAMAGE COLLIDER
        else if(player.playerNetworkManager.isUsingLeftHand.Value)
        {
            leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }
        // PLAY WHOOSH SFX
        
    }
}