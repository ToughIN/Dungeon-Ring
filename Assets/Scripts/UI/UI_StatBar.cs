using System.Collections;
using System.Collections.Generic;
using ToufFrame;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider slider;
    private RectTransform rectTransform;
    public UI_Canvas_PlayerHUD PlayerHUDCanvas 
    {get=>UIMgr.Instance.canvasDIC[GameStrings.ADDRESSABLES_UI_CANVAS_PlayerHUD] as UI_Canvas_PlayerHUD;}

    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithStats=true;
    [SerializeField] protected float widthScaleMultiplier=1f;
    
    // SECEONDARY BAR BEHIND MAY BAR FOR POLISH EFFECT (YELLOW BAR THAT SHOWS HOW MUCH AN ACTION/DAMAGE TAKE AWAY FROM CURRENT STAT)

    protected virtual void Awake()
    {
        rectTransform=GetComponent<RectTransform>();
        slider=GetComponent<Slider>();
    }

    public virtual void SetStat(int newValue)
    {
        slider.value = newValue;
    }
    
    public virtual void SetMaxStat(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarLengthWithStats)
        {
            rectTransform.sizeDelta=new Vector2(maxValue*widthScaleMultiplier,rectTransform.sizeDelta.y);

            PlayerHUDCanvas.uiPanelHudBasic.RefreshHUD();
        }
        
        
    }

}
