using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToufFrame;
public class WorldSoundFXManager : MonoSingletonBase<WorldSoundFXManager>
{
    [Header("Action Sounds")] 
    public AudioClip rollSFX;

    [Header("Damage Sounds")]
    public AudioClip[] physicalDamageSFX;
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
    }
    
    public AudioClip ChooseRandomSFXFromArray(AudioClip[] sfxArray)
    {
        return sfxArray[UnityEngine.Random.Range(0, sfxArray.Length)];
    }
}
