using System;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    private CharacterManager character;
    
    [Header("Stamina Regeneration")] 
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] private float staminaRegenerationDelay = 2;
    [SerializeField] private float staminaRegenerationAmount = 2;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected void Start()
    {
        
    }

    public int CalculateHealthBasedOnVitalityLevel(int vitality)
    {
        float health = 0;
        
        // Create An Equation for how you want your health to be calculated;
        health = vitality * 10;

        return Mathf.RoundToInt(health);
    }
    
    public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0;
        
        // Create An Equation for how you want your samina to be calculated;
        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
    }
    public virtual void RegenerateStamina()
    {
        //ONLY OWNERS CAN EDIT THEIR NETWORK VARIABLES
        if(!character.IsOwner)return;
        //WE DONT WANT TO REGENERATE STAMINA IF WE ARE USING IT
        if (character.characterNetWorkManager.isSprinting.Value)return;
        if(character.isPerformingAction)return;
        
        staminaRegenerationTimer+= Time.deltaTime;
        
        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (character.characterNetWorkManager.currentStamina.Value < character.characterNetWorkManager.maxStamina.Value)
            {
                staminaTickTimer += staminaTickTimer + Time.deltaTime;
                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.characterNetWorkManager.currentStamina.Value += staminaRegenerationAmount;
                }
            }
            
        }
    }

    public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        //WE ONLY WANT TO RESET THE REGENERATION IF THE ACTION USED STAMINA
        //WE DONT WANT TO RESET THE REGENERATION IF WE ARE ALREADY REGENERATING
        if (currentStaminaAmount < previousStaminaAmount)
        {
            staminaRegenerationTimer = staminaRegenerationDelay;
        }
    }
}