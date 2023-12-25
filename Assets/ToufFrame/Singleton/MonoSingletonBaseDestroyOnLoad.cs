using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ToufFrame
{
    /// <summary>
    /// Singleton base class, instance automatically added and thread-safe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingletonBaseDestroyOnLoad<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();

                        Debug.Log($"[MonoSingletonBase] An instance of {typeof(T)} was created: {_instance.gameObject.name}");
                    }
                    else
                    {
                        Debug.Log($"[MonoSingletonBase] Using existing instance of {typeof(T)}: {_instance.gameObject.name}");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"[MonoSingletonBase] A second instance of {typeof(T)} was attempted to be created and will be destroyed: {gameObject.name}");
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                // Debug.Log($"[MonoSingletonBase] {typeof(T)} instance has been destroyed: {gameObject.name}");
            }
        }
    }
}
