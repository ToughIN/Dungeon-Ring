using System.Collections;
using System.Collections.Generic;
using ToufFrame;
using UnityEngine;
using UnityEngine.Serialization;

public class WorldCharacterEffectsManager : MonoSingletonBase<WorldCharacterEffectsManager>
{
    [SerializeField] private List<SO_InstantCharacterEffect> instantEffects;

    [Header("Damage")] 
    public SO_TakeDamageEffect takeDamageEffect;
    
    [Header("VFX")] 
    public GameObject bloodSplatterVFX;
    
    protected override void Awake()
    {
        base.Awake();
        GenerateEffectIDs();
    }

    private void GenerateEffectIDs()
    {
        for (int i = 0; i < instantEffects.Count; i++)
        {
            instantEffects[i].instantEffectID = i;
        }
    }

}
