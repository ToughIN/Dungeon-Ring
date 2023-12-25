using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using ToufFrame;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public partial class UI_Panel_HUD_Basic : BasePanel
{
    //[Header("STAT BARS")]
    private UI_StatBar staminaBar;
    private UI_StatBar healthBar;

    //[Header("QUICK SLOTS")] 
    private Image rightWeaponQuickSlotIcon;
    private Image leftWeaponQuickSlotIcon;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        staminaBar = BUC_Slider_StaminaBar.GetComponent<UI_StatBar>();
        healthBar = BUC_Slider_HealthBar.GetComponent<UI_StatBar>();

        rightWeaponQuickSlotIcon = BUC_Image_RightWeapon_Icon.image;
        leftWeaponQuickSlotIcon = BUC_Image_LeftWeapon_Icon.image;
    }

    
    

    public void RefreshHUD()
    {
        healthBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(false);
        
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(true);
    }
    
    public void SetNewStaminaValue(float oldValue,float newValue)
    {
        staminaBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxStaminaValue(float maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }
    
    public void SetNewHealthValue(float oldValue,float newValue)
    {
        healthBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxHealthValue(float maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }

    public void SetRightWeaponQuickSlotIcon(int weaponID)
    {
        
        SO_WeaponSoItem weaponSO=WorldItemDataBase.Instance.GetWeaponByID(weaponID);
        if (weaponSO == null)
        {
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if (weaponSO.itemIcon == null)
        {
            Debug.Log("ITEM ICON IS NULL");
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }
        
        // this is where you would check to see if you meet the items requirements if you want to create the warning for not being able to wield it in the ui
        
        rightWeaponQuickSlotIcon.sprite=weaponSO.itemIcon;
        rightWeaponQuickSlotIcon.enabled = true;
    }

    public void SetLeftWeaponQuickSlotIcon(int weaponID)
    {
        SO_WeaponSoItem weaponSO=WorldItemDataBase.Instance.GetWeaponByID(weaponID);
        if (weaponSO == null)
        {
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if (weaponSO.itemIcon == null)
        {
            Debug.Log("ITEM ICON IS NULL");
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }
        
        // this is where you would check to see if you meet the items requirements if you want to create the warning for not being able to wield it in the ui
        
        leftWeaponQuickSlotIcon.sprite=weaponSO.itemIcon;
        leftWeaponQuickSlotIcon.enabled = true;
    }
    
}
