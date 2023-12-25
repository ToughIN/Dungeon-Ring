using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character_Save_Slot : MonoBehaviour
{
    SaveFileDataWriter saveFileDataWriter;
    
    [Header("Game Slot")]
    public ECharacterSlots characterSlot;

    [Header("Character Info")] 
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timePlayed;
    public Image characterPortrait;

    private void OnEnable()
    {
        if(characterSlot!=ECharacterSlots.NoSlot)
            LoadSaveSlots();
    }

    private void LoadSaveSlots()
    {
        saveFileDataWriter= new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = GameStrings.PATH_SAVE_FILE_DIRECTORY;
        
        saveFileDataWriter.saveFileName=WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCharacterSlosBeingUsed(characterSlot);
        if (saveFileDataWriter.CheckToSeeIfSaveFileExists())
        {
            characterName.text = WorldSaveGameManager.Instance.characterSlots[(int)characterSlot].charqacterName;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    public void LoadGameFromCharacterSlot()
    {
        if (characterSlot == ECharacterSlots.NoSlot)
        {
            //TODO:START NEW GAME
            return;
        }
        WorldSaveGameManager.Instance.currentCharacterSlotsBeingUsed = characterSlot;
        WorldSaveGameManager.Instance.LoadGame();
    }

    public void SelectCurrentSlot()
    {
        TitleScreenManager.Instance.SelectCharacterSlot( characterSlot);
    }
}






//
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
//
// public class UI_Character_Save_Slot : MonoBehaviour
// {
//     SaveFileDataWriter saveFileDataWriter;
//     
//     [Header("Game Slot")]
//     public ECharacterSlots characterSlot;
//
//     [Header("Character Info")] 
//     public TextMeshProUGUI characterName;
//     public TextMeshProUGUI timePlayed;
//
//     private void OnEnable()
//     {
//         LoadSaveSlots();
//     }
//
//     private void LoadSaveSlots()
//     {
//         saveFileDataWriter= new SaveFileDataWriter();
//         saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
//         
//         saveFileDataWriter.saveFileName=WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCharacterSlosBeingUsed(characterSlot);
//         if (saveFileDataWriter.CheckToSeeIfSaveFileExists())
//         {
//             characterName.text = WorldSaveGameManager.Instance.characterSlots[(int)characterSlot].charqacterName;
//         }
//         else
//         {
//             gameObject.SetActive(false);
//         }
//         
//     }
//
//     public void LoadGameFromCharacterSlot()
//     {
//         WorldSaveGameManager.Instance.currentCharacterSlotsBeingUsed = characterSlot;
//         WorldSaveGameManager.Instance.LoadGame();
//     }
//
//     public void SelectCurrentSlot()
//     {
//         TitleScreenManager.Instance.SelectCharacterSlot( characterSlot);
//     }
// }