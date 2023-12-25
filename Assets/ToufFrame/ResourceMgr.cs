using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ToufFrame
{
    public class ResourceMgr : SingletonBase<ResourceMgr>

    {
        /// <summary>
        /// Synchronous Resource Loading
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string name) where T : Object
        {
            T res = Resources.Load<T>(name);
            // If object is GameObject, Instantiate it and return
            if (res is GameObject)
            {
                return GameObject.Instantiate(res);
            }
            else
            {
                return res;
            }
            
        }

        /// <summary>
        /// Asynchronous load resource
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback">The function called after the resource has been fully loaded, you can use lambda function here</param>
        /// <typeparam name="T"></typeparam>
        public void LoadAsync<T>(string name,UnityAction<T> callback) where T : Object
        {
            MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(name,callback));
        }

        /// <summary>
        /// Really function to start asynchronous coroutine 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerator ReallyLoadAsync<T>(string name,UnityAction<T> callback) where T : Object
        {
            ResourceRequest r = Resources.LoadAsync<T>(name);
            yield return r;
            
            if(r.asset is GameObject)
                callback(GameObject.Instantiate(r.asset) as T);
            else
            {
                callback(r.asset as T);
            }
        }
    
    }
}