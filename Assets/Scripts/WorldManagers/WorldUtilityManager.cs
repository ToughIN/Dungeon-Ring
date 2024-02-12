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
    
    public bool CanIDamageThisTarget(ECharacterGroup attackingCharacter,ECharacterGroup targetCharacter)
    {
        if (attackingCharacter == targetCharacter)
            return false;
        if (attackingCharacter == ECharacterGroup.Team01 && targetCharacter == ECharacterGroup.Team01)
            return false;
        if (attackingCharacter == ECharacterGroup.Team02 && targetCharacter == ECharacterGroup.Team02)
            return false;
        return true;
    }

    /// <summary>
    ///  获取目标的角度
    /// </summary>
    /// <param name="characterTransform"></param>
    /// <param name="targetDirection"></param>
    /// <returns></returns>
    public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
    {
        targetDirection.y = 0;
        float viewableAngle= Vector3.Angle(targetDirection, characterTransform.forward);
        Vector3 cross = Vector3.Cross(targetDirection, characterTransform.forward);
        if(cross.y<0)
            viewableAngle = -viewableAngle;
        return viewableAngle;
    }
    
     
    
    
}