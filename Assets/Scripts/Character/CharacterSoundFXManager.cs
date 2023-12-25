using System;
using UnityEngine;
using Random = System.Random;

public class CharacterSoundFXManager : MonoBehaviour
{

    private AudioSource audioSource;

    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundFX(AudioClip soundFX,float volume=1,bool randomizePitch = true,float pitchRandom=0.1f)
    {
        audioSource.PlayOneShot(soundFX,volume);

        audioSource.pitch = 1;
        if (randomizePitch)
        {
            audioSource.pitch+= UnityEngine.Random.Range(-pitchRandom,pitchRandom);
        }
    }   

    public void PlayRollSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.Instance.rollSFX);
    }
}