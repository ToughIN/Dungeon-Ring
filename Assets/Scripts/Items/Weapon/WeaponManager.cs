using System;
using ToufFrame;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public MeleeWeaponDamageCollider meleeDamageCollider;

    private void Awake()
    {
        meleeDamageCollider= GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon,SO_WeaponSoItem soWeaponSo)
    {
        meleeDamageCollider.characterCausingDamage = characterWieldingWeapon;
        meleeDamageCollider.physicalDamage = soWeaponSo.physicalDamage;
        meleeDamageCollider.magicalDamage = soWeaponSo.magicDamage;
        meleeDamageCollider.fireDamage = soWeaponSo.fireDamage;
        meleeDamageCollider.lightningDamage = soWeaponSo.lightningDamage;
        meleeDamageCollider.holyDamage = soWeaponSo.holyDamage;

        meleeDamageCollider.light_Attack_01_Modifier = soWeaponSo.light_Attack_01_Modifier;
        meleeDamageCollider.light_Attack_02_Modifier = soWeaponSo.light_Attack_02_Modifier;
        meleeDamageCollider.heavy_Attack_01_Modifier = soWeaponSo.heavy_Attack_01_Modifier;
        meleeDamageCollider.charge_Attack_01_Modifier = soWeaponSo.charge_Attack_01_Modifier;

    }
}