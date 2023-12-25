using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;

namespace ToufFrame
{
    public partial class BasePanel : MonoBehaviour
    {
        protected UIMgr uiMgr;
        public virtual string ResourceName { get; set; }
        //string存储物体名，List<UIBehaviour>存储物体上的组件
        protected Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();
        private CanvasGroup canvasGroup;
        [HideInInspector]public BaseCanvas rootCanvas;

        

        protected virtual void OnEnable()
        {
            uiMgr = UIMgr.Instance;
        }

        protected void OnDestroy()
        {
        }

        protected T GetControl<T>(string controlname) where T : UIBehaviour
        {
            if (controlDic.ContainsKey(controlname))
            {
                for(int i =0; i<controlDic[controlname].Count; i++)
                {
                    if(controlDic[controlname][i] is T)
                    {
                        return controlDic[controlname][i] as T;
                    }
                }
            }

            return null;
        }
        

        public virtual void OnClick(string name)
        {
            
        }
        
        public void Fade(float startAlpha,float endAlpha,float duration)
        {
            canvasGroup.alpha = startAlpha;
            StartCoroutine(FadeCanvasGroup(canvasGroup, startAlpha, endAlpha, duration));
        }

        public void FadeIn(float duration)
        {
            StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, duration));
        }

        public void FadeOut(float duration)
        {
            StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, duration));
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 1)
        {
            float _timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / lerpTime;

            while (true)
            {
                timeSinceStarted=Time.time-_timeStartedLerping;
                percentageComplete=timeSinceStarted/lerpTime;
                
                float currentValue = Mathf.Lerp(start,end,percentageComplete);

                cg.alpha = currentValue;
                
                if(percentageComplete>1)break;
                yield return new WaitForEndOfFrame();
            }
        }
        
        
    }
}

// protected void FindChildControl<T>() where T : UIBehaviour
// {
//     T[] controls = this.GetComponentsInChildren<T>();
//             
//     for (int i = 0; i < controls.Length; i++)
//     {
//         string objName = controls[i].gameObject.name;
//         if(controlDic.ContainsKey(objName))
//         {
//             controlDic[objName].Add(controls[i]);
//         }
//         else
//         {
//             controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
//         }
//
//         if (controls[i] is Button)
//         {
//             (controls[i] as Button).onClick.AddListener(() =>
//             {
//                 OnClick(objName);
//             });
//         }
//         else if(controls[i] is Toggle)
//         {
//             (controls[i] as Toggle).onValueChanged.AddListener((value) =>
//             {
//                 OnValueChanged();
//             });
//         }
//         else if(controls[i] is Slider)
//         {
//             (controls[i] as Slider).onValueChanged.AddListener((value) =>
//             {
//                 OnValueChanged();
//             });
//         }
//         else if(controls[i] is ScrollRect)
//         {
//             (controls[i] as ScrollRect).onValueChanged.AddListener((value) =>
//             {
//                 OnValueChanged();
//             });
//         }
//         else if(controls[i] is InputField)
//         {
//             (controls[i] as InputField).onValueChanged.AddListener((value) =>
//             {
//                 OnValueChanged();
//             });
//         }
//     }
// }
//
// protected virtual void Awake()
// {
//     canvasGroup = GetComponent<CanvasGroup>();
//     if (canvasGroup == null)
//     {
//         canvasGroup = gameObject.AddComponent<CanvasGroup>();
//     }
//     FindChildControl<Button>();
//     FindChildControl<Image>();
//     FindChildControl<Text>();
//     FindChildControl<Toggle>();
//     FindChildControl<Slider>();
//     FindChildControl<ScrollRect>();
//     FindChildControl<InputField>();
//     FindChildControl<TextMeshProUGUI>();  // 添加对TextMeshProUGUI的支持
//             
// }