using System;
using System.Collections;
using System.Collections.Generic;
using ToufFrame;
using Unity.Netcode;
using UnityEngine;


public class WorldAIManager : MonoSingletonBase<WorldAIManager>
{
    [Header("Characters")] 
    [SerializeField] private List<AICharacterSpawner> aiCharacterSpawners = new List<AICharacterSpawner>();
    [SerializeField] private List<GameObject> spawnedAICharacters = new List<GameObject>();
    [SerializeField] private TriggerVariable<bool> ifRespawn = new TriggerVariable<bool>(false);

    
    
    private void Start()
    {
        ifRespawn.OnValueChange += (b =>
        {
            if (b)
            {
            }
            else DespawnAllAICharacter();
        });
    }

    private IEnumerator WaitForSceneToLoadThenSpawnAI()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            aiCharacterSpawners.Add(aiCharacterSpawner);
            aiCharacterSpawner.AttemptToSpawnCharacter();
        };
    }
    
    
    private void DespawnAllAICharacter()
    {
        foreach (var character in spawnedAICharacters)
        {
            character.GetComponent<NetworkObject>().Despawn(true);
        }
    }

    private void DisableAllCharacters()
    {
        
    }
}