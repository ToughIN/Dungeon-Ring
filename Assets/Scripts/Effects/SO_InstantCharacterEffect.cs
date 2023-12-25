using UnityEngine;

public class SO_InstantCharacterEffect : ScriptableObject
{
    [Header("Effect ID")] public int instantEffectID;

    public virtual void ProcessEffect(CharacterManager character)
    {
        
    }

}