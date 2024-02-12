using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterNetWorkManager : NetworkBehaviour
{
    private CharacterManager character;
    
    [Header("Position")]
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.one, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public Vector3 networkPositionVelocity = Vector3.zero;
    
    public float networkPositionSmoothTime= 0.2f;
    public float networkRotationSmoothTime= 0.2f;

    [Header("Target")] public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>();
    
    [Header("Animator")]
    public NetworkVariable<bool> isMoving=new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")] 
    public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isChargingAttack= new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> maxStamina = new NetworkVariable<float>( 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> maxHealth = new NetworkVariable<float>( 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats")]
    [SerializeField]public NetworkVariable<int> vitality = new NetworkVariable<int>(50, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField]public NetworkVariable<int> endurance = new NetworkVariable<int>(50, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void CheckHP(float oldValue, float newValue)
    {
        if (currentHealth.Value <= 0)
        {
            StartCoroutine(character.ProcessDeathEvent());
        }

        if (character.IsOwner)
        {
            if (currentHealth.Value > maxHealth.Value)
            {
                currentHealth.Value = maxHealth.Value;
            }
        }
    }

    public void OnIsMovingChanged(bool oldStatus, bool newStatus)
    {
        character.animator.SetBool(GameStrings.VARIABLE_ANIMATOR_IsMoving, isMoving.Value);
    }
    
    public void OnLockOnTargetIDChange(ulong oldID,ulong newID)
    {
        if (!IsOwner)
        {
            character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID]
                .GetComponent<CharacterManager>();
        }
    }

    public void OnIsLockedOnChanged(bool old, bool isLockedOn)
    {
        if (!isLockedOn)
        {
            character.characterCombatManager.currentTarget = null;
        }
    }
    
    public void OnIsChargingAttackChanged(bool oldStatus,bool newStatus)
    {
        character.animator.SetBool(GameStrings.VARIABLE_ANIMATOR_IsChargingAttack,isChargingAttack.Value);
    }

    // A SERVER RPC IS A FUNCTION CALLED FROM A CLIENT TO THE SERVER
    [ServerRpc]
    public void NotifyTheServerOfActionAnimationServerRPC(ulong clientID, string animationID, bool applyRootMotion)
    {
        //IF THIS IS THE SERVER, THEN ACTIVATE THE CLIENT RPC
        if (IsServer)
        {
            PlayActionAnimationForAllClientsClientRPC(clientID,animationID,applyRootMotion);
        }
    }
    
    
    
    
    // A CLIENT RPC IS SENT TO ALL CLIENTS PRESENT. FROM THE SERVER
    [ClientRpc]
    public void PlayActionAnimationForAllClientsClientRPC(ulong clientID,string animationID, bool applyRootMotion)
    {
        //WE MAKE SURE TO NOT RUN THE FUNCTION ON THE CHARACTER WHO SENT IT
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAnimationFromServer(animationID,applyRootMotion);
        }
    }
    
    // ATTACK ANIMATION

    [ServerRpc]
    public void NotifyTheServerOfAttackActionAnimationServerRPC(ulong clientID, string animationID, bool applyRootMotion)
    {
        //IF THIS IS THE SERVER, THEN ACTIVATE THE CLIENT RPC
        if (IsServer)
        {
            PlayAttackActionAnimationForAllClientsClientRPC(clientID,animationID,applyRootMotion);
        }
    }

    [ClientRpc]
    public void PlayAttackActionAnimationForAllClientsClientRPC(ulong clientID,string animationID, bool applyRootMotion)
    {
        //WE MAKE SURE TO NOT RUN THE FUNCTION ON THE CHARACTER WHO SENT IT
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAnimationFromServer(animationID,applyRootMotion);
        }
    }
    
    private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
    {
        character.characterAnimatorManager.applyRootMotion=applyRootMotion;
        character.animator.CrossFade(animationID,0.2f);
    }
    
    // DAMAGE
    [ServerRpc]
    public void NotifyTheServerOfCharacterDamageServerRPC(ulong damagedCharacterID,ulong characterCausingDamageID,float physicalDamage,
        float magicDmage, float holyDamage, float poiseDamage, float angleHitFrom, float contactPointX,float contactPointY,float contacktPointZ)
    {
        if (IsServer)
        {
            NotifyTheServerOfCharacterDamageClientRPC(damagedCharacterID,characterCausingDamageID,physicalDamage,magicDmage,holyDamage,poiseDamage,angleHitFrom,contactPointX,contactPointY,contacktPointZ);
        }
    }
    
    [ClientRpc]
    public void NotifyTheServerOfCharacterDamageClientRPC(ulong damagedCharacterID,ulong characterCausingDamageID,float physicalDamage,
        float magicDmage, float holyDamage, float poiseDamage, float angleHitFrom, float contactPointX,float contactPointY,float contacktPointZ)
    {
        ProcessDamageFromServer( damagedCharacterID, characterCausingDamageID, physicalDamage,
            magicDmage,  holyDamage,  poiseDamage,  angleHitFrom,  contactPointX, contactPointY, contacktPointZ);
    }
    
    public void ProcessDamageFromServer(ulong damagedCharacterID,ulong characterCausingDamageID,float physicalDamage,
        float magicDmage, float holyDamage, float poiseDamage, float angleHitFrom, float contactPointX,float contactPointY,float contacktPointZ)
    {
        CharacterManager damagedCharacter=NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager
            .SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();
        SO_TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect);
        
        damageEffect.holyDamage = holyDamage;
        damageEffect.magicDamage = magicDmage;
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.angleHitFrom = angleHitFrom;
        damageEffect.contactPoint=new Vector3(contactPointX,contactPointY,contacktPointZ);
        damageEffect.characterCausingDamage = characterCausingDamage;
        
        damagedCharacter.characterEffectcManager.ProcessInstantEffect(damageEffect);
    }

    
}