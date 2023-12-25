using System;
using UnityEngine;

public class PlayerEffectsManager: CharacterEffectcManager
{
    [Header("Debug Delete Later")] [SerializeField]
    private SO_InstantCharacterEffect effectToTest;

    [SerializeField] private bool processEffect = false;

    private void Update()
    {
        if (processEffect)
        {
            processEffect = false;
            SO_InstantCharacterEffect effect = Instantiate(effectToTest);
            ProcessInstantEffect(effect);
        }
    }

    public virtual void ProcessInstantEffect(SO_InstantCharacterEffect effect)
    {
    }
        
}