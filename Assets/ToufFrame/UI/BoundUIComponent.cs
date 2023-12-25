using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace ToufFrame
{
    public class BoundUIComponent : MonoBehaviour
    {
        public Button button;
        public Image image;
        public Text text;
        public Toggle toggle;
        public Slider slider;
        public ScrollRect scrollRect;
        public InputField inputField;
        public TextMeshProUGUI textMeshProUGUI;
        public Scrollbar scrollbar;
        public BaseControl customControl;
        
        public void GetContrls()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            text = GetComponent<Text>();
            toggle = GetComponent<Toggle>();
            slider = GetComponent<Slider>();
            scrollRect = GetComponent<ScrollRect>();
            inputField = GetComponent<InputField>();
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            scrollbar = GetComponent<Scrollbar>();
            customControl = GetComponent<BaseControl>();
        }
                
    }
     
}