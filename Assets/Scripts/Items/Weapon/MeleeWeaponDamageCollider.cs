using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")] public CharacterManager characterCausingDamage;

    [Header("Weapon Attack Modifiers")] 
    public float light_Attack_01_Modifier=1f;
    public float light_Attack_02_Modifier = 1.2f;
    public float heavy_Attack_01_Modifier = 2f;
    public float charge_Attack_01_Modifier = 3f;

    protected override void Awake()
    {
        base.Awake();
        damageCollider.enabled = false;
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //TODO: CHANGE THE LAYER TO A MORE SPECIFIC ONE
        CharacterManager damageTarget = other.GetComponent<CharacterManager>();

        
        if (damageTarget != null)
        {
            if(damageTarget==characterCausingDamage)
                return;
            
            contactPoint = other.gameObject.GetComponentInParent<Collider>().ClosestPointOnBounds(transform.position);
            // if you want to search on both the damageable character colliders & the character controller collider just check for null here and do the following:
            if (damageTarget == null)
            {
                damageTarget= other.GetComponent<CharacterManager>();
            }
            
            
            //check if we can damage this target based on friendly fire
            
            //check if target is blocking
            
            // check if target is invulnerable
            
            DamageTarget(damageTarget);
            
        }
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if(characterDamaged.Contains(damageTarget))
            return;
        characterDamaged.Add(damageTarget);
        
        SO_TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicalDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case EAttackType.LightAttack01:
                ApplyAttackDamageModifiers(light_Attack_01_Modifier,damageEffect);
                break;
            case EAttackType.LightAttack02:
                ApplyAttackDamageModifiers(light_Attack_02_Modifier,damageEffect);
                break;
            case EAttackType.HeavyAttack01:
                ApplyAttackDamageModifiers(heavy_Attack_01_Modifier,damageEffect);
                break;
            case EAttackType.ChargedAttack01:
                ApplyAttackDamageModifiers(charge_Attack_01_Modifier,damageEffect);
                break;
            default:
                break;
        }
        
        damageTarget.characterEffectcManager.ProcessInstantEffect(damageEffect);
        if (characterCausingDamage.IsOwner)
        {
            damageTarget.characterNetWorkManager.NotifyTheServerOfCharacterDamageServerRPC(
                damageTarget.NetworkObjectId,
                characterCausingDamage.NetworkObjectId,
                damageEffect.physicalDamage,
                damageEffect.magicDamage,
                damageEffect.holyDamage,
                damageEffect.poiseDamage,
                damageEffect.angleHitFrom,
                damageEffect.contactPoint.x,
                damageEffect.contactPoint.y,
                damageEffect.contactPoint.z);
        }
        
    }

    private void ApplyAttackDamageModifiers(float modifier,SO_TakeDamageEffect damage)
    {
        damage.physicalDamage *= modifier;
        damage.magicDamage *= modifier;
        damage.fireDamage *= modifier;
        damage.holyDamage *= modifier;
        damage.poiseDamage *= modifier;
        
        
    }
}

