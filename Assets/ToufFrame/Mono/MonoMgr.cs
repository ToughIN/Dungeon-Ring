using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

namespace ToufFrame
{
    /// <summary>
    /// Provide mono support for some scripts not inherit from MonoBehaviour
    /// </summary>
    public class MonoMgr : SingletonBase<MonoMgr>
    {
        public MonoController controller;
        
        public MonoMgr()
        {
            //保证了MonoController对象的唯一性
            GameObject obj = new GameObject("MonoController");
            controller = obj.AddComponent<MonoController>();
        }
        
        public void AddUpdateListener(UnityAction fun)
        {
            
            controller.AddUpdateListener(fun);

           
        }


        public void RemoveUpadateListener(UnityAction fun)
        {
            controller.RemoveUpadateListener(fun);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return controller.StartCoroutine(routine);

        }

        public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
        {
            return controller.StartCoroutine(methodName, value);
        }

        public Coroutine StartCoroutine(string methodName)
        {
            return controller.StartCoroutine(methodName);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            controller.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            controller.StopCoroutine(routine);
        }
        
        
    }
}