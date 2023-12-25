using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ToufFrame
{
    public class ScenesMgr :SingletonBase<ScenesMgr>
    {

        
        /// <summary>
        ///  
        /// </summary>
        public void LoadScene(string name,UnityAction fun)
        {
            SceneManager.LoadScene(name);

            fun();
        }


        /// <summary>
        /// Provides loading interfaces for external async loading scenes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fun">the method called after scene loaded</param>
        public void LoadSceneAsyn(string name, UnityAction fun)
        {
            MonoMgr.Instance.StartCoroutine(IELoadSceneAsyn(name, fun));
        }

        
        /// <summary>
        /// Coroutines for asynchronous loading scenes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        private IEnumerator IELoadSceneAsyn(string name, UnityAction fun)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(name);

            while (!ao.isDone)
            {
                //TODO: Find somewhere to create the event about progress bar
                EventMgr.Instance.TriggerEvent("Update the progress bar",ao.progress);
                yield return ao.progress;
            }
            
            
            
            fun();
        }
        
    }
}