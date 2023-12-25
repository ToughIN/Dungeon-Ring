using System;
using UnityEngine;

public class CharacterEffectcManager : MonoBehaviour
{
    //PROCESS INSTANCE EFFECTS
    
    //PROCESS TIMED EFFECTS
    
    //PROCESS STATIC EFFECTS

    private CharacterManager character;

    [Header("VFX")] 
    [SerializeField] private GameObject bloodSplatterVFX;
    
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(SO_InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        // if we manually have placed a blood splatter vfx on this model, play its version
        if (bloodSplatterVFX != false)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX,contactPoint, Quaternion.identity);
        }
        // else, use the generic (Default Version) we have elsewhere
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.Instance.bloodSplatterVFX,contactPoint, Quaternion.identity);
            
        }
    }
        
}