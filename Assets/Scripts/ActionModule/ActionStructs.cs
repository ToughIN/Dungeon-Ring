
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionModule
{
    
    public struct ActionInfo
    {
        public string Id { get; set; }                 // 动作的唯一标识符
        public string AnimKey { get; set; }            // 动画名称
        public string[] Catalog { get; set; }         // 动作分类
        public CancelTag[] CancelTag { get; set; }       // 这个动作可以取消其他动作的依据
        public BeCancelledTag[] BeCancelledTag { get; set; }  // 这动作通常可以被其他动作取消的依据
        public TempBeCancelledTag[] TempBeCancelledTag { get; set; }  // 因为攻击会临时开启的Cancel信息
        public ActionCommand[] Commands { get; set; }  // 触发动作的命令数组
        public MoveInputAcceptance[] InputAcceptance { get; set; }     // 允许的移动倍率
        public string AutoNextActionId { get; set; }   // 自动下一个动作的ID
        public float Priority { get; set; }            // 动作优先级
        public bool KeepPlayingAnim { get; set; }      // 动作切换到自己时是否保持播放
        public bool AutoTerminate { get; set; }        // 动作是否自动终止
        public AttackInfo[] Attacks { get; set; }      // 动作期间的攻击信息
        public AttackBoxTurnOnInfo[] AttackPhase { get; set; } // 攻击盒开启阶段
        public BeHitBoxTurnOnInfo[] BeAttackPhase { get; set; }   // 受击盒开启阶段
        public ScriptMethodInfo RootMotionTween  { get; set; }      // 动作唯一信息，拿出来的RootMotion的信息，只提供移动的参考，不直接控制移动
        // 其他必要属性
    }

    
    public struct CancelTag
    {
        public string Tag { get; set; }                // 动作标签,如果BeCancelledTag的tag中包含了这个tag，说明这个CancelInfo就可以导致这个动作在“上一个动作”的这个BeCancelledTag生效的阶段内替换了它。
        public float StartFrom { get; set; }           // 取消时从哪一帧开始，因为使用animator，所以应该是一个百分比
        public float BlendIn { get; set; }             // 动作融合时间
        public int Priority { get; set; }              // 优先级调整
    }
    
    
    public struct BeCancelledTag
    {
        public string[] Tags { get; set; }            // 动作标签,盒cancleTag的Tag对应，一个BeCancelledTag是可以被多个CancelTag所cancel的，同样的多个BeCancelledTag也可能被同一个CancelTag所Cancel
        public Range ActiveRange { get; set; }        // 生效范围
        public float BlendOut { get; set; }           // 当前动作融合出去的时间
        public int Priority { get; set; }             // 优先级变化
    }
    
    
    /// <summary>
    /// 在攻击发生后，临时开启的Cancel点，是一个短暂的Cancle点，不是一早就开启的
    /// </summary>
    public struct TempBeCancelledTag
    {
        public string Id { get; set; }                // 增，因为需要被开启，所以需要一个ID
        public string[] Tags { get; set; }            
        public float ActiveRange { get; set; }        //改，因为是临时开启，无法定义启动时间，但需要一个多久来帮助获得结束时间
        public float BlendOut { get; set; }           
        public int Priority { get; set; }                         
    }
    
   
    public struct Range
    {
        public float Min { get; set; } // 生效范围的最小值
        public float Max { get; set; } // 生效范围的最大值

        public Range(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
    
    /// <summary>
    /// 一段攻击
    /// </summary>
    public struct AttackInfo
    {
        public int Phase { get; set; }                 // 攻击阶段，如果一个动作有多个攻击阶段，每个阶段的phase应该唯一
        public float AttackPower { get; set; }         // 攻击力倍数
        public float HitStun { get; set; }             // 硬直时间，对手侧停顿多久
        public float Freeze { get; set; }              // 卡帧时间，自己停顿多久
        public MoveInfo PushPower { get; set; }        // 击退力
        public int CanHitSameTarget { get; set; }      // 可命中同一目标的次数
        public float HitSameTargetDelay { get; set; }  // 命中同一目标的间隔时间，如果可以多次命中，每次之间的“冷却时间”
        public ActionChangeInfo SelfActionChange { get; set; } // 攻击发生时，攻击者自身动作的变化信息
        public ActionChangeInfo TargetActionChange { get; set; } // 攻击发生时，被攻击者动作的变化信息
        public string[] TempBeCancelledTagTurnOn { get; set; } // 攻击发生时，临时开启的TempBeCancelledTag的Id,可以实现只有命中后的派生
        // 其他必要属性
    }

    
    /// <summary>
    /// 不同帧开启的攻击盒无论是位置、数量还是储存都是不同的
    /// </summary>
    public struct AttackBoxTurnOnInfo
    {
        public Range Range { get; set; }               // 生效的时间范围,在一段时间开启一批攻击盒，在这段时间让攻击盒的工作有意义
        public string[] Tag { get; set; }             // 攻击盒的标签,开启的对应的攻击盒
        public int AttackPhase { get; set; }           // 与attackInfo的phase对应
        public int Priority { get; set; }              // 优先级
    }
    
    public struct BeHitBoxTurnOnInfo
    {
        public Range Range { get; set; }               // 生效的时间范围
        public string[] Tag { get; set; }             // 受击盒的标签
        public int Priority { get; set; }              // 优先级
        public string[] TempBeCancelledTagTurnOn { get; set; } // 受击盒开启时，临时开启的TempBeCancelledTag的Id
        public ActionChangeInfo SelfActionChange { get; set; } // 受击盒开启时，双方动作变化
    }

    public struct MoveInputAcceptance
    {
        public Range Range { get; set; }               // 生效的时间范围
        public float Rate { get; set; }                // 接受的百分比
    }

    /// <summary>
    /// RootMotion的信息，只提供移动的参考，不直接控制移动
    /// </summary>
    public struct ScriptMethodInfo
    {
        public string Method { get; set; }             // 要调用的函数名，各种方法调用。指向RootMotionMethod类下唯一一个静态dictionary Methods的key，由此可以拿到value就是delegate了。
        public string[] Params { get; set; }           // 函数参数，配表
    }

    public struct ActionCommand
    {
        public InputAction[] InputActions { get; set; }  // 使用InputAction
        public float ValidTime { get; set; }             // 命令有效时间
    }
    
    public struct MoveInfo
    {
        public Vector3 Direction { get; set; }  // 击退或击飞的方向
        public float Force { get; set; }        // 力度（推力）
        public float Duration { get; set; }     // 持续时间

        public MoveInfo(Vector3 direction, float force, float duration)
        {
            Direction = direction;
            Force = force;
            Duration = duration;
        }
    }
    
    public struct ActionChangeInfo
    {
        public string NextActionId { get; set; }  // 下一个动作的ID
        public float BlendTime { get; set; }      // 动作融合时间
        public MoveInfo? MovementChange { get; set; } // 动作切换时的移动变化（可选）
        public string AnimationOverride { get; set; } // 动画覆盖，例如改变为特殊状态

        public ActionChangeInfo(string nextActionId, float blendTime, MoveInfo? movementChange, string animationOverride)
        {
            NextActionId = nextActionId;
            BlendTime = blendTime;
            MovementChange = movementChange;
            AnimationOverride = animationOverride;
        }
    }

}