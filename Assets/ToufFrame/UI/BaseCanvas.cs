using System.Collections.Generic;
using UnityEngine;
using ToufFrame;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class BaseCanvas : MonoBehaviour
{
    public virtual string ResourceName { get; set; }
    
    protected UIMgr uiMgr;
    private Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();
    
    public RectTransform bottom;
    public RectTransform middle;
    public RectTransform top;
    public RectTransform system;
    
    public Canvas canvas;
    
    public virtual void Init()
    {
        if(bottom == null)
            bottom = new GameObject("Bottom").AddComponent<RectTransform>();
        if (middle == null)
            middle = new GameObject("Middle").AddComponent<RectTransform>();
        if(top == null)
            top = new GameObject("Top").AddComponent<RectTransform>();
        if(system == null)
            system = new GameObject("System").AddComponent<RectTransform>();
        if(canvas == null)
             canvas = gameObject.AddComponent<Canvas>();
        uiMgr = UIMgr.Instance;
    }

    
    

}