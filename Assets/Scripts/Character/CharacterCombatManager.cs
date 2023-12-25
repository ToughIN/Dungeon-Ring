
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterCombatManager : NetworkBehaviour
{
    private CharacterManager character;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;
    
    [Header("Attack Target")]
    public CharacterManager currentTarget;
    
    [Header("Attack Type")]
    public EAttackType currentAttackType;
    
    [FormerlySerializedAs("currentWeaponSoUsed")] [FormerlySerializedAs("currentWeaponUsed")] public SO_WeaponSoItem currentSoWeaponSoUsed;
    
    
    [Header("Locked On Transform")]
    public Transform lockedOnTransform;
    

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (character.IsOwner)
        {
            if (newTarget != null)
            {
                currentTarget = newTarget;
                character.characterNetWorkManager.currentTargetNetworkObjectID.Value= newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                
            }
            else
            {
                currentTarget = null;
                
            }
            
        }
        
    }

}