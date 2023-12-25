#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ToufFrame
{
    public class PoolData
    {
        public GameObject enablePoolObj;
        public GameObject disablePoolObj;
        public List<GameObject> poolList;

        public PoolData(GameObject obj, GameObject enablePoolObj,GameObject disablePoolObj)
        {
            this.enablePoolObj = new GameObject(obj.name);
            this.disablePoolObj = new GameObject(obj.name);
            this.enablePoolObj.transform.SetParent(enablePoolObj.transform);
            this.disablePoolObj.transform.SetParent(disablePoolObj.transform);
            poolList = new List<GameObject>() { obj };
            PushObj(obj);
        }


        public void PushObj(GameObject obj)
        {
            obj.SetActive(false);
            poolList.Add(obj);
            obj.transform.SetParent(disablePoolObj.transform);
        }


        public void GetObj(string name, UnityAction<GameObject> callBack)
        {
            if (poolList.Count > 0)
            {
                GameObject obj = poolList[0];
                poolList.RemoveAt(0);
                obj.transform.SetParent(enablePoolObj.transform);
                obj.SetActive(true);
                callBack(obj);
            }
            else
            {
                switch (UIMgr.Instance.resourceLoadType)
                {
                    case EResourceLoadType.Resources:
                        LoadByResources(name, callBack);
                        break;
                    case EResourceLoadType.Addressable:
                        break;
                }
            }
        }

        private void LoadByResources(string name,UnityAction<GameObject> callBack)
        {
            ResourceMgr.Instance.LoadAsync<GameObject>(name,(obj) =>
            {
                // obj.name = name;
                Debug.Log(ToufNames.FRAME_DEBUG_LOG+"[PoolMgr] new "+name+" successfully added to pool");
                if (obj == null)
                {
                    Debug.Log(ToufNames.FRAME_DEBUG_LOG+"[PoolMgr] Can't find "+name+".prefab in 'Reources' folder");
                    callBack(null);
                    return;
                }
                obj.transform.SetParent(enablePoolObj.transform);
                obj.SetActive(true);
                callBack(obj);
            });
        }

        private void LoadByAddressables(string name,UnityAction<GameObject> callBack)
        {
            AddressablesAssetManager.Instance.LoadAssetAsync<GameObject>(name, (obj) =>
            {
                // obj.name = name;
                Debug.Log(ToufNames.FRAME_DEBUG_LOG+"[PoolMgr] new "+name+" successfully added to pool");
                if (obj == null)
                {
                    Debug.Log(ToufNames.FRAME_DEBUG_LOG+"[PoolMgr] Can't find "+name+".prefab in 'Reources' folder");
                    callBack(null);
                    return;
                }
                obj.transform.SetParent(enablePoolObj.transform);
                obj.SetActive(true);
                callBack(obj);
            });
        }
        
    }
    
    
    
    //
    //
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // public class PoolMgr : MonoSingletonBase<PoolMgr>
    // {
    //     /// <summary>
    //     /// dictionary of pools
    //     /// </summary>
    //     public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //
    //     private GameObject RootPoolObj;
    //     private GameObject EnabledPoolObj;
    //     private GameObject DisabledPoolObj;
    //     
    //
    //     /// <summary>
    //     ///  
    //     /// </summary>
    //     /// <param name="name">name of the needed prefab in "Resources" folder </param>
    //     /// <returns></returns>
    //     public void GetObj(string name,UnityAction<GameObject> callBack=null)
    //     {
    //         if (RootPoolObj == null)
    //         {
    //             RootPoolObj = new GameObject($"{ToufNames.FRAME_NAME} pool");
    //             EnabledPoolObj = new GameObject("EnabledPool");
    //             DisabledPoolObj = new GameObject("DisabledPool");
    //             EnabledPoolObj.transform.SetParent(RootPoolObj.transform);
    //             DisabledPoolObj.transform.SetParent(RootPoolObj.transform);
    //         }
    //         if (poolDic.ContainsKey(name))
    //         {
    //            poolDic[name].GetObj(name,callBack);
    //         }
    //         else
    //         {
    //             //obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
    //             ResourceMgr.Instance.LoadAsync<GameObject>(name,(o) =>
    //             {
    //                 o.name = name;
    //                 if (o == null)
    //                 {
    //                     Debug.Log(ToufNames.FRAME_DEBUG_LOG+"[PoolMgr] Can't find "+name+".prefab in 'Reources' folder");
    //                 }
    //                 poolDic.Add(name,new PoolData(o,EnabledPoolObj,DisabledPoolObj));
    //                 // o = poolDic[name].GetObj(name,callBack);
    //                 callBack(o);
    //             });
    //             
    //             
    //         }
    //
    //     }
    //
    //     
    //     /// <summary>
    //     /// 
    //     /// </summary>
    //     /// <param name="name">name of the needed prefab in "Resources" folder</param>
    //     /// <param name="obj"></param>
    //     public void PushObj(string name, GameObject obj)
    //     {
    //         if (RootPoolObj == null)
    //         {
    //             RootPoolObj = new GameObject($"{ToufNames.FRAME_NAME} pool");
    //             EnabledPoolObj = new GameObject("EnabledPool");
    //             DisabledPoolObj = new GameObject("DisabledPool");
    //             EnabledPoolObj.transform.SetParent(RootPoolObj.transform);
    //             // DisabledPoolObj.transform.SetParent(RootPoolObj.transform);
    //         }
    //
    //         obj.transform.SetParent(DisabledPoolObj.transform);
    //         
    //         if (poolDic.ContainsKey(name))
    //         {
    //             poolDic[name].PushObj(obj);
    //         }
    //         else
    //         {
    //             poolDic.Add(name,new PoolData(obj,EnabledPoolObj,DisabledPoolObj));
    //         }
    //     }
    //
    //
    //
    //     
    //
    //     /// <summary>
    //     /// automatically called when the scene change
    //     /// </summary>
    //     private void Clear(Scene scene, LoadSceneMode mode)
    //     {
    //         poolDic.Clear();
    //         RootPoolObj = null;
    //         EnabledPoolObj = null;
    //         DisabledPoolObj = null;
    //         
    //     }
    //
    //     protected override void Awake()
    //     {
    //         base.Awake();
    //         SceneManager.sceneLoaded += Clear;
    //     }
    //     
    //
    //    
    //     
    // }
}