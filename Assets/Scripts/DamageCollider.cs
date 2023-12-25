using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")] 
    [SerializeField]protected Collider damageCollider;
    
    
    [Header("Damage")] 
    public float physicalDamage = 0;
    public float magicalDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")] protected List<CharacterManager> characterDamaged = new List<CharacterManager>();

    protected virtual void Awake()
    {
        damageCollider = GetComponent<Collider>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //TODO: CHANGE THE LAYER TO A MORE SPECIFIC ONE
        CharacterManager damageTarget = other.GetComponent<CharacterManager>();

        if (damageTarget != null)
        {
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

    protected virtual void DamageTarget(CharacterManager damageTarget)
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

        damageTarget.characterEffectcManager.ProcessInstantEffect(damageEffect);


    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        // Debug.Log("clear ");
        characterDamaged.Clear();
    }
    
}