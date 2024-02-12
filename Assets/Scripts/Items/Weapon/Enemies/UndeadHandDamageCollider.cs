using UnityEngine;
using UnityEngine.Serialization;

public class UndeadHandDamageCollider : DamageCollider
{
    [SerializeField] private AICharacterManager undeadCharacter;


    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponent<Collider>();
        undeadCharacter = GetComponentInParent<AICharacterManager>();
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
        damageEffect.angleHitFrom = Vector3.SignedAngle(undeadCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

        switch (undeadCharacter.characterCombatManager.currentAttackType)
        {
            
        }
        
        damageTarget.characterEffectcManager.ProcessInstantEffect(damageEffect);
        
        //
        if (damageTarget.IsOwner)
        {
            damageTarget.characterNetWorkManager.NotifyTheServerOfCharacterDamageServerRPC(
                damageTarget.NetworkObjectId,
                undeadCharacter.NetworkObjectId,
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

}