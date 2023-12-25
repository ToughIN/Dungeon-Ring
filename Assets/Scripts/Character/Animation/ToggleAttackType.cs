using UnityEngine;

public class ToggleAttackType : StateMachineBehaviour
{
    private CharacterManager character;

    [SerializeField] private EAttackType attackType;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }

        character.characterCombatManager.currentAttackType = attackType;
    }
}