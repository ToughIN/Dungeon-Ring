using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class SO_WeaponSoItem : SO_Item
{
    [Header("Weapon Model")] public GameObject weaponModel;

    [Header("Weapon Requirements")] public int strengthREQ = 0;
    public int dexREQ = 0;
    public int intREQ = 0;
    public int faithREQ = 0;

    [Header("Weapon Base Damage")] public int physicalDamage = 0;
    public int magicDamage = 0;
    public int fireDamage = 0;
    public int holyDamage = 0;
    public int lightningDamage = 0;
    
    //WEAPON MODIFIERS
    [Header("Weapon Base Poise")] public float poiseDamage = 10;
    
    [Header("Attack Modifiers")]
    public float light_Attack_01_Modifier = 1.1f;
    public float light_Attack_02_Modifier = 1.2f;
    public float heavy_Attack_01_Modifier = 1.5f;
    public float charge_Attack_01_Modifier = 2f;
    
    
    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
    public float lightAttackStaminaCostMultiplier = 0.9f;

    [Header("Actions")] 
    public WeaponItemAction commonAttack; // RB || LMB ,light Attack or release magic
    [FormerlySerializedAs("on_RT_Action")] public WeaponItemAction heavyAttack; // HeavyAttack || shift+LMB
    
    
    
    


}