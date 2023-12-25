using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// SINCE WE WANT TO REFERENCE THIS DATA FOR EVERY SAVE FILE, THIS SCRIPT IS NOT A MONOBEHAVIOUR IS INSTEAD SERIALIZABLE
public class CharacterSaveData
{
    [Header("SCENE INDEX")]
    public int sceneIndex=1;
    
    [Header("Character Name")]
    public string charqacterName="Tav";
    
    
    [Header("Time Played")]
    public float secondsPlayed;

    [Header("World Coordinates")] 
    public float xPosition;
    public float yPosition;
    public float zPosition;
    
    [Header("Resources")]
    public  float currentStamina;
    public float currentHealth;
    
    
    
    [Header("Stats")]
    public int vitality;
    public int endurance;
    
    
    [Header("Inventory")]
    public List<string> items_SO_Addressable_PathList=new List<string>();
}