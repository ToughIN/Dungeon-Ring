using System.Collections;
using System.Collections.Generic;
using ToufFrame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ToufFrame
{

    public interface IEventInfo
    {
        
    }
    
    
    public class EventInfo<T1, T2> : IEventInfo
    {
        public UnityAction<T1, T2> actions;

        public EventInfo(UnityAction<T1, T2> action)
        {
            actions += action;
        }
    }
    
    public class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> actions;
        public EventInfo(UnityAction<T> action)
        {
            actions += action;
        }
    }

    public class EventInfo : IEventInfo
    {
        public UnityAction actions;
        public EventInfo(UnityAction action)
        {
            actions += action;
        }
    }
    
    
    
    public class EventMgr : MonoSingletonBase<EventMgr>
    {
        /// <summary>
        /// string - event name
        /// 
        /// </summary>
        private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();


        
        public void AddEventListener<T1, T2>(string name, UnityAction<T1, T2> action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T1, T2>).actions += action;
            }
            else
            {
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is created");
                eventDic.Add(name, new EventInfo<T1, T2>(action));
            }
        }
        
        
        /// <summary>
        /// 为一个事件添加监听，如果事件不存在则创建
        /// </summary>
        /// <param name="name">name of event</param>
        /// <param name="action"></param>
        public void AddEventListener<T>(string name, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T>).actions += action;
            }
            else
            {
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is created");
                eventDic.Add(name,new EventInfo<T>(action));
            }
        }
        
        public void AddEventListener(string name, UnityAction action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo).actions += action;

            }
            else
            {
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is created");
                eventDic.Add(name,new EventInfo(action));
            }
        }
        
        
        
        public void RemoveEventListener<T1, T2>(string name, UnityAction<T1, T2> action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T1, T2>).actions -= action;
            }
            else
            {
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is not existed, remove failed");
            }
        }
        
        
        /// <summary>
        /// RemoveLisener
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void RemoveEventListener<T>(string name, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(name))
            {
                
                (eventDic[name] as EventInfo<T>).actions -= action;
                
            }
            else
            {
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is not existed, remove failed");
            }
        }
        
        public void RemoveEventListener(string name, UnityAction action)
        {
            if (eventDic.ContainsKey(name))
            {
                if (eventDic.ContainsKey(name))
                {
                    (eventDic[name] as EventInfo).actions -= action;
                }
            }
            else
            {
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is not existed, remove failed");
            }
        }

        public void RemoveEvent(string name)
        {
            if (eventDic.ContainsKey(name))
            {
                eventDic.Remove(name);
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Event '{name}' is completely removed.");
            }
            else
            {
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Event '{name}' does not exist, remove failed.");
            }
        }


        public void TriggerEvent<T1, T2>(string name, T1 info1, T2 info2)
        {
            if (eventDic.ContainsKey(name))
            {
                if ((eventDic[name] as EventInfo<T1, T2>).actions != null)
                    (eventDic[name] as EventInfo<T1, T2>).actions.Invoke(info1, info2);
            }
            else
            {
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is not existed, trigger failed");
            }
        }
        

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="info">事件信息,传入参数</param>
        public void TriggerEvent<T>(string name,T info)
        {
            if (eventDic.ContainsKey(name))
            {
                if((eventDic[name] as EventInfo<T>).actions != null)
                    (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            }
            else
            {
                
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is not existed, trigger failed");
            }
        }

        public void TriggerEvent(string name)
        {
            if (eventDic.ContainsKey(name))
            {
                if ((eventDic[name] as EventInfo).actions != null)
                    (eventDic[name] as EventInfo).actions.Invoke();
            }
            else
            {
                
                Debug.Log($"{ToufNames.FRAME_DEBUG_LOG}[EventMgr] Action '{name} is not existed, trigger failed");
            }
        }
        
        
        /// <summary>
        /// 重新单例基类的Clear方法，场景变换时被调用
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        public  void Clear(Scene scene, LoadSceneMode mode)
        {
            eventDic.Clear();
            
        }

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += Clear;
        }
        
        
    }
    
}