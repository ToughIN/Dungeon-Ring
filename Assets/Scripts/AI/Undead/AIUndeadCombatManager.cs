using UnityEngine;

public class AIUndeadCombatManager : AICharacterManager
{
    [Header("damage colliders")]
    [SerializeField] UndeadHandDamageCollider rightHandDamageCollider;
    [SerializeField] UndeadHandDamageCollider leftHandDamageCollider;

    [Header("Damage")] 
    [SerializeField] private int baseDamage = 25;
    [SerializeField] private float attack01DamageModifier = 1.0f;
    [SerializeField] private float attack02DamageModifier = 1.5f;

    public void SetAttack01Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }
    
    public void SetAttack02Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
    }
    
    public void OpenRightHandDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }
    
    public void CloseRightHandDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }
    
    public void OpenLeftHandDamageCollider()
    {
        leftHandDamageCollider.EnableDamageCollider();
    }
    
    public void CloseLeftHandDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
    }
    


}