using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace ToufFrame
{
    
    
    public class UIMgr : MonoSingletonBase<UIMgr>
    {
        public bool IfUseInputSystem = true;
        
        public EResourceLoadType resourceLoadType = EResourceLoadType.Addressable;
        
        public Dictionary<string,BasePanel> panelDIC = new Dictionary<string, BasePanel>();

        public Dictionary<string,BaseCanvas> canvasDIC = new Dictionary<string, BaseCanvas>();
        
        private GameObject eventSystemObj;
        
        protected override void Awake()
        {
            base.Awake();
            InitEventSystem();
        }
        
        private void InitEventSystem()
        {
            if ( eventSystemObj==null && GameObject.Find("EventSystem") == null)
            {
                eventSystemObj = new GameObject("EventSystem");
                if (IfUseInputSystem)
                {
                    eventSystemObj.AddComponent<EventSystem>();
                    eventSystemObj.AddComponent<InputSystemUIInputModule>();
                }
                GameObject.DontDestroyOnLoad(eventSystemObj);
            }
        }


        /// <summary>
        /// ShowPanel
        /// </summary>
        /// <param name="panelName">UI预制体的名字</param>
        /// <param name="layer"></param>
        /// <param name="callBack">the function you wanna called after the panel was created</param>
        /// <typeparam name="T">type of the panel inherit PanelBase</typeparam>
        public void ShowPanel<T>(string canvasName,string panelName,EUIlayer layer=EUIlayer.Mid,UnityAction<T> callBack=null) where T : BasePanel
        {
            BaseCanvas canvas = null;
            if (!canvasDIC.TryGetValue(canvasName, out canvas))
            {
                Debug.LogWarning( "canvas not exist");
                return;
            }
            if (panelDIC.ContainsKey(panelName))
            {
                if(callBack!=null)
                    callBack(panelDIC[panelName] as T);
                return;
            }
            
            //TODO:when the panel has not been fully loaded, but was called again, the callBack will be called twice
            switch (resourceLoadType)
            {
                case EResourceLoadType.Resources:
                {
                    ResourceMgr.Instance.LoadAsync<GameObject>(ToufNames.UI_RESOURCES_PATH + panelName, (obj) =>
                    {
                        LoadPanelAsync<T>(canvas,panelName,obj,layer,callBack);
                    });
                }
                    break;
                case EResourceLoadType.Addressable :
                {
                    Addressables.LoadAssetAsync<GameObject>(panelName).Completed += (obj) =>
                    {
                        GameObject go = Instantiate(obj.Result);
                        LoadPanelAsync<T>(canvas,panelName,go,layer,callBack);
                    };
                }
                    break;
                default:
                    Debug.LogError("resourceLoadType not exist");
                    break;
            }
        }
    
        

        public void ClosePanel(string panelName)
        {
            
            if (panelDIC.ContainsKey(panelName))
            {
                if (resourceLoadType == EResourceLoadType.Resources)
                {
                    Addressables.Release(panelDIC[panelName].gameObject);
                }
                GameObject.Destroy(panelDIC[panelName].gameObject);
                panelDIC.Remove(panelName);
            }
            else
            {
                Debug.LogError(ToufNames.FRAME_DEBUG_LOG+$"[{panelName}]"+" panel not exist");
            }
        }

        public T GetPannel<T>(string name) where T : BasePanel
        {
            if (panelDIC.ContainsKey(name))
                return panelDIC[name] as T;
            return null;
        }
  
        /// <summary>
        /// Add custom event listening
        /// </summary>
        /// <param name="control">the ui object</param>
        /// <param name="type">event type</param>
        /// <param name="callBack">the function when the event triggerrd</param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type,UnityAction<BaseEventData> callBack)
        {
            EventTrigger trigger = control.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = control.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callBack);
            trigger.triggers.Add(entry);
        }
        
        private void LoadPanelAsync<T>(BaseCanvas canvas,string panelName,GameObject panelObj,EUIlayer layer,UnityAction<T> callBack=null) where T : BasePanel
        {
            Transform father = canvas.middle;

            switch (layer)
            {
                case EUIlayer.Mid:
                    father = canvas.middle;
                    break;
                case EUIlayer.Top:
                    father = canvas.top;
                    break;
                case EUIlayer.System:
                    father = canvas.system;
                    break;
                default:
                    father = canvas.bottom;
                    break;
            }

            panelObj.transform.SetParent(father);

            panelObj.transform.localPosition = Vector3.zero;
            panelObj.transform.localScale = Vector3.one;

            (panelObj.transform as RectTransform).offsetMax = Vector2.zero;
            (panelObj.transform as RectTransform).offsetMin = Vector2.zero;

            BasePanel panel = panelObj.GetComponent<BasePanel>();
            if (callBack != null)
            {
                callBack(panel as T);
            }
            
            Debug.Log(panelName);
            panelDIC.Add(panelName, panel);
            panel.rootCanvas = canvas;

        }
        
        private void LoadCanvasAsync(string canvasName,GameObject canvasObj,UnityAction<BaseCanvas> callBack)
        {
            canvasObj.transform.localPosition = Vector3.zero;
            canvasObj.transform.localScale = Vector3.one;
            (canvasObj.transform as RectTransform).offsetMax = Vector2.zero;
            (canvasObj.transform as RectTransform).offsetMin = Vector2.zero;
            BaseCanvas canvas = canvasObj.GetComponent<BaseCanvas>();
            if (callBack != null)
            {
                callBack(canvas);
            }
            canvasDIC.Add(canvasName, canvas);
        }
        
        private void LoadCanvasAsync(string canvasName,UnityAction<BaseCanvas> callBack)
        {
            if (canvasDIC.ContainsKey(canvasName))
            {
                callBack(canvasDIC[canvasName]);
                return;
            }
            switch (resourceLoadType)
            {
                case EResourceLoadType.Resources:
                {
                    ResourceMgr.Instance.LoadAsync<GameObject>(ToufNames.UI_RESOURCES_PATH + canvasName, (obj) =>
                    {
                        LoadCanvasAsync(canvasName,obj,callBack);
                    });
                }
                    break;
                case EResourceLoadType.Addressable :
                {
                    Addressables.LoadAssetAsync<GameObject>(canvasName).Completed += (obj) =>
                    {
                        LoadCanvasAsync(canvasName,obj.Result,callBack);
                    };
                }
                    break;
                default:
                    Debug.LogError("resourceLoadType not exist");
                    break;
            }
        }
        
    }
}