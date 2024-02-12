using System;
using System.Collections;
using System.Collections.Generic;
using ToufFrame;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead=new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    public bool testIsOwner = true;
    
    [HideInInspector]public CharacterController characterController;
    [HideInInspector]public Animator animator;
    
    [HideInInspector]public CharacterNetWorkManager characterNetWorkManager;
    [HideInInspector]public CharacterEffectcManager characterEffectcManager;
    [HideInInspector]public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
    [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
    
    [Header("Character Group")]
    public ECharacterGroup characterGroup;
    
    
    [Header("Flags")]
    public bool isPerformingAction=false;
    

    
    
    protected  virtual void Awake()
    {
        
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetWorkManager = GetComponent<CharacterNetWorkManager>();
        characterEffectcManager = GetComponent<CharacterEffectcManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
        characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
    }

    protected virtual void Start()
    {
        IgmoreMyOwnColliders();
        if (IsOwner)
        {
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded+= OnSceneLoaded;
        }
        
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void Update()
    {
        //If this character is being controlled from our side, then assign its network position the to the position of our transform
        // Debug.Log(transform.name+" "+IsOwner);
        if (IsOwner||testIsOwner)
        {
            characterNetWorkManager.networkPosition.Value = transform.position;
            characterNetWorkManager.networkRotation.Value = transform.rotation;
        }
        //If this character is being controlled from else where, then assigh its position here locally
        else
        {
            
            //POSITION
            transform.position = Vector3.SmoothDamp(transform.position,characterNetWorkManager.networkPosition.Value,
                ref characterNetWorkManager.networkPositionVelocity,characterNetWorkManager.networkPositionSmoothTime);
            
            //ROTATION
            transform.rotation=Quaternion.Slerp(transform.rotation,characterNetWorkManager.networkRotation.Value,
                Time.deltaTime*characterNetWorkManager.networkRotationSmoothTime);
        }
    }

    protected virtual void FixedUpdate()
    {
        // throw new NotImplementedException();
    }

    protected virtual void LateUpdate()
    {
        
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        characterNetWorkManager.OnIsMovingChanged(false,characterNetWorkManager.isMoving.Value);
        characterNetWorkManager.isMoving.OnValueChanged+= characterNetWorkManager.OnIsMovingChanged;
    } 

    public virtual IEnumerator ProcessDeathEvent(bool    manuallySelectDeathAnimation= false)
    {
        if (IsOwner)
        {
            characterNetWorkManager.currentHealth.Value = 0;
            isDead.Value = true;
            
            // reset any flags here that need to be reset
            // 
            
            // if we are not grounded, lay an aerial death animation
            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01",true);
            }
        }

        yield return new WaitForSeconds(0);
        
        //award players with runes
        //disable character
        
        
    }

    public virtual void ReviveCharacter()
    {
        
    }

    protected virtual void IgmoreMyOwnColliders()
    {
        Collider characterControllerCollider= GetComponent<Collider>();
        Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
        
        List<Collider>ignoreColliders=new List<Collider>();

        foreach (var collider in damageableCharacterColliders)
        {
            ignoreColliders.Add(collider);
        }
        
        ignoreColliders.Add(characterControllerCollider);

        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider,otherCollider,true);
            }
        }
    }
    
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPositionOnSceneLoaded();
    }
    
    protected virtual void SetPositionOnSceneLoaded()
    {
        this.transform.position = new Vector3(0, 0, 0);

    }
}