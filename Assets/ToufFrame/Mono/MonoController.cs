using System;
using UnityEngine;
using UnityEngine.Events;

namespace ToufFrame
{
    /// <summary>
    /// Provide mono support for some scripts not inherit from MonoBehaviour
    /// </summary>
    public class MonoController : MonoBehaviour
    {
        private event UnityAction updateEvent;
        
        
        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (updateEvent != null)
            {
                updateEvent();
            }
            
        }

        public void AddUpdateListener(UnityAction fun)
        {
            updateEvent += fun;
        }


        public void RemoveUpadateListener(UnityAction fun)
        {
            updateEvent -= fun;
        }
        
    }
}