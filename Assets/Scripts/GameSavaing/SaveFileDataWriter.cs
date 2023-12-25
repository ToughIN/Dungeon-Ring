using System;
using System.IO;
using UnityEngine;

public class SaveFileDataWriter 
{
    public string saveDataDirectoryPath = GameStrings.PATH_SAVE_FILE_DIRECTORY;
    public string saveFileName = "";
        
    public bool CheckToSeeIfSaveFileExists()
    {
        if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
        {
            return true;
        }
        else
        {
            return false;    
        }
            
    }

    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath,saveFileName));
    }

    //Save
    public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
    { 
        string savePath=Path.Combine(saveDataDirectoryPath,saveFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("CREATING SAVE FILE, AT SAVE PATH: " + savePath);

            string dataToStore = JsonUtility.ToJson(characterData, true);
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError("ERROR WHILST TRYING TO SAVE CAHRACTER DATA, GAME NOT SAVED "+savePath+"\n"+ex);
        }
       
    }
    
    //Load
    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterData = null;
        
        string loadPath=Path.Combine(saveDataDirectoryPath,saveFileName);

        try
        {
            if (File.Exists(loadPath))
            {
                string dataToLoad = "";
            
                using(FileStream stream=new FileStream(loadPath,FileMode.Open))
                {
                    using(StreamReader reader=new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
        }
        catch (Exception ex)
        {
            characterData=null;
            Debug.Log(" ERROR LOADING SAVE FILE, GAME NOT LOADED "+loadPath+"\n"+ex);
        }
        return characterData;

    }
    
    
}