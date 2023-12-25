using ToufFrame;
using UnityEngine;

public class WorldUtilityManager : MonoSingletonBase<WorldUtilityManager>
{
    [SerializeField] private LayerMask characterLayers;
    [SerializeField] private LayerMask environmentLayers;
    
    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }
    
    public LayerMask GetEnvironmentLayers()
    {
        return environmentLayers;
    }
}