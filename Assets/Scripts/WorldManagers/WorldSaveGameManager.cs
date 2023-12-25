using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ToufFrame;
public class WorldSaveGameManager : MonoSingletonBase<WorldSaveGameManager>
{

    [SerializeField]public PlayerManager player;

    [Header("Save/Load")] 
    [SerializeField] private bool saveGame;
    [SerializeField] private bool loadGame;


    [Header("World Scene Index")]
    [SerializeField]private int worldSceneIndex = 1;

    [Header("Save Data Writer")] 
    private SaveFileDataWriter saveFileDataWriter;
    
    [Header("Current Character Data")]
    public ECharacterSlots currentCharacterSlotsBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;
    
    [Header("Character Slots")]
    [SerializeField]public Dictionary<int,CharacterSaveData> characterSlots=new Dictionary<int, CharacterSaveData>();

    private void Start()
    {
        LoadAllCharacterProfiles();
        
    }

    private void Update()
    {
        if(saveGame)
        {
            saveGame = false;
            SaveGame();
        }
        if(loadGame)
        {
            loadGame = false;
            LoadGame();
        }
            
    }

    public string DecideCharacterFileNameBasedOnCharacterSlosBeingUsed(ECharacterSlots characterSlot)
    {
        string fileName = $"CharacterSlot_{(int)characterSlot+1}";
        return fileName;
    }

    public void AttemptToCreateNewGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = GameStrings.PATH_SAVE_FILE_DIRECTORY;
        
        for (int i = (int)ECharacterSlots.CharacterSlot1; i <= (int)ECharacterSlots.CharacterSlot10; i++)
        {
            if(!Enum.IsDefined(typeof(ECharacterSlots), i))continue;
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlosBeingUsed((ECharacterSlots)i);
            if (!saveFileDataWriter.CheckToSeeIfSaveFileExists())
            {
                currentCharacterSlotsBeingUsed = (ECharacterSlots)i;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }
        }
        
        player.playerNetworkManager.endurance.Value = 20;
        player.playerNetworkManager.vitality.Value = 20;
        
        TitleScreenManager.Instance.DisplayNoFreeCharacterSlotsPopUp();
    }

    private void NewGame()
    {
        
        //SAVES THE NEWLY CREATED CHARACTER STATS, AND ITEMS
        player.playerNetworkManager.vitality.Value = 15; 
        player.playerNetworkManager.endurance.Value = 15;
        
        SaveGame();
        StartCoroutine(LoadWorldScene());
    }

    public void LoadGame()
    {
        saveFileName= DecideCharacterFileNameBasedOnCharacterSlosBeingUsed(currentCharacterSlotsBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        // GENERALLY WORKS ON MULTIPLE MACHINE TYPES
        saveFileDataWriter.saveDataDirectoryPath = GameStrings.PATH_SAVE_FILE_DIRECTORY;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData= saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame()
    {
        saveFileName= DecideCharacterFileNameBasedOnCharacterSlosBeingUsed(currentCharacterSlotsBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        
        saveFileDataWriter.saveDataDirectoryPath = GameStrings.PATH_SAVE_FILE_DIRECTORY;
        saveFileDataWriter.saveFileName = saveFileName;
        
        // PASS THE PLAYERS INFO, FROM GAME, TO THEIR SAVE FILE
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);
        // WRITE THAT INFO ON A JSON FILE
        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public void DeleteGame(ECharacterSlots characterSlot)
    {
        saveFileDataWriter= new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath=GameStrings.PATH_SAVE_FILE_DIRECTORY;
        saveFileDataWriter.saveFileName= DecideCharacterFileNameBasedOnCharacterSlosBeingUsed(characterSlot);

        
        
        saveFileDataWriter.DeleteSaveFile();
    }
    
    
    
    //LOAD ALL CHARACTER PROFILES ON DEVICE WHEN STARTING GAME
    private void LoadAllCharacterProfiles()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = GameStrings.PATH_SAVE_FILE_DIRECTORY;

        for (int i = (int)ECharacterSlots.CharacterSlot1; i <= (int)ECharacterSlots.CharacterSlot10; i++)
        {
            if(!Enum.IsDefined(typeof(ECharacterSlots), i))continue;
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlosBeingUsed((ECharacterSlots)i);
            if (saveFileDataWriter.CheckToSeeIfSaveFileExists())
            {
                characterSlots[i] = saveFileDataWriter.LoadSaveFile();
            }
            else
            {
                characterSlots[i] = null;
            }
        }
        
    }
    
    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperator = SceneManager.LoadSceneAsync(worldSceneIndex);
        // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);
        
        player.LoadGameFromCurrentCharacterData(ref currentCharacterData);
        
        
        yield return null;
    }

    // private IEnumerator LoadWorldSceneNewGame()
    // {
    //     
    // }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
    
    
}
