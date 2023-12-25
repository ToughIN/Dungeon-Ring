using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class SO_TakeDamageEffect : SO_InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;//if the damage is caused by another characters attack it will be stored here

    [Header("Damage")] 
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Aniamtion")] public bool playDamageAnimation = true;
    public bool manuallySetDamageAnimation = false;
    public string damageAnimation;

    [Header("Poise")] public float poiseDamage = 0;
    public bool poiseIsBroken = false;
    
    [Header("Final Damage")]
    private float finalDamageDealt = 0;

    [Header("Sound FX")] public bool willPlayDamageSFX = true;// USED ON TOP OF REGULAR SFX IF THERE IS ELEMENTAL DAMAGE PRESENT
    public AudioClip elementalDamageSoundFX;

    [Header("Direction Damage Taken From")]
    public float angleHitFrom;// USED TO DETERMINE WHAT DAMAGE ANIMATION TO PLAY
    public Vector3 contactPoint;//BLOOD FX
    
    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);
        if (character.isDead.Value)
        {
            return;
        }
        // check for invulnerability
        
        // calculate damage
        CalculateDamage(character);
        
        //check which directional damage came from
        
        PlayDirectionalBasedDamageAnimation(character);
        
        //check for build ups
        
        //check for build ups
        //play damage sound fx
        PlayDamageVFX(character);
        PlayDamageSFX(character);
        //if character is ai, check for new target if character causing damage is present
        
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner)
        {
            return;
        }
        
        if (characterCausingDamage != null)
        {
            //check for damage modifiers and modify base damage
        }
        //check character for flat defenses and subtract them from the damage
        
        //check character for armor absorptions, and subtract the percentage from the damage
        
        //add all damage types together, and apply final damage
        finalDamageDealt=Mathf.RoundToInt(physicalDamage+magicDamage+fireDamage+lightningDamage+holyDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        character.characterNetWorkManager.currentHealth.Value -= finalDamageDealt;
        
        //calculate poise damage to determine if the character will be stunned
        
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        // if we have fire damage, play fire particles
        // lighting damage, lighting particles ect
        
        character.characterEffectcManager.PlayBloodSplatterVFX(contactPoint);
        
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        AudioClip physicalDamageSFX = WorldSoundFXManager.Instance.ChooseRandomSFXFromArray(WorldSoundFXManager.Instance.physicalDamageSFX);
        
        character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
    }
    
    private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner)
        {
            return;   
        }
        if(character.isDead.Value)
        {
            return;
        }
        // TODD: CALCULATE IF POISE IS BROKEN
        poiseIsBroken = true;
        
        if (angleHitFrom > 145 && angleHitFrom <= 180)
        {
            damageAnimation=
                character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager
                    .forward_Medium_Damage);
        }
        else if (angleHitFrom <= 145 && angleHitFrom >= -180)
        {
            damageAnimation =
                character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager
                    .forward_Medium_Damage);
        }
        else if (angleHitFrom >= -45 && angleHitFrom <= -45)
        {
            damageAnimation =
                character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager
                    .bakcward_Medium_Damage);
        }
        else if (angleHitFrom >= -145 && angleHitFrom <= -45)
        {
            damageAnimation =
                character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager
                    .left_Medium_Damage);
        }
        else if (angleHitFrom >= 45 && angleHitFrom <= 145)
        {
            damageAnimation =
                character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager
                    .right_Medium_Damage);
        }
        
        
        // IF POISE IS BROKEN, PLAY A STAGGERING DAMAGE ANIMATION
        if (poiseIsBroken)
        {
            character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
            character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation,true);
        }
        

    }
    
}