using ToufFrame;
using TMPro;
public class UI_Control_ItemDescription_Scroll : BaseControl
{
    public TextMeshProUGUI textMeshProUGUI;
    
    public void SetText(string text)
    {
        textMeshProUGUI.text = text;
    }

    public void ShowDescription(SO_Item item)
    {
        textMeshProUGUI.text = item.itemDescription;
    }
    
    
    
    
}