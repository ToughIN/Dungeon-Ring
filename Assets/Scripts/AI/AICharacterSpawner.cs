using System;
using Unity.Netcode;
using UnityEngine;

public class AICharacterSpawner : MonoBehaviour
{
    [Header("Characer")] 
    [SerializeField] private GameObject characterGameObject;

    [SerializeField] private GameObject instantiatedGameobject;


    private void Awake()
    {
        
    }

    private void Start()
    {
        WorldAIManager.Instance.SpawnCharacter(this);
        gameObject.SetActive(false);
    }
    
    public void AttemptToSpawnCharacter()
    {
        if (characterGameObject!=null)
        {
            instantiatedGameobject = Instantiate(characterGameObject);
            instantiatedGameobject.transform.position = transform.position;
            instantiatedGameobject.transform.rotation = transform.rotation;
            instantiatedGameobject.GetComponent<NetworkObject>().Spawn();
        }
    }
    
    
}