
    using UnityEngine;

    [CreateAssetMenu(menuName = "AI/Action/Attack")]
    public class AICharacterAttackAction : ScriptableObject
    {
        [Header("Action Animation")]
        [SerializeField] private string attackAnimation; //这个攻击的动画
        
        
        [Header("Combo Action")] 
        public AICharacterAttackAction comboAction;//这个攻击的连击攻击
        
        [Header("Action Values")]
        [SerializeField] EAttackType attackType; //这个攻击的类型
        public int attackWeight=50; //这个攻击的权重
        //攻击类型
        //是否重复
        public float actionRecoveryTime = 1.5f;//可以进行下一次攻击的时间
        public float minimumAttackAngle = -35;
        public float maximumAttackAngle = 35;
        public float maximumAttackDistance = 2;
        public float minimumAttackDistance = 0;
        
        
        public void AttemptToPerformAction(AICharacterManager aiCharacter)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation(attackAnimation,true);
        }
        
    }
