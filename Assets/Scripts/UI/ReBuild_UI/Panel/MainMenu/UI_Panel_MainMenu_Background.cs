using ToufFrame;
using UnityEngine.AddressableAssets;

public partial class UI_Panel_MainMenu_Background : BasePanel
{
    public override string ResourceName { get=>GameStrings.ADDRESSABLES_UI_PANEL_Background;  }
    BoundUIComponent Text_Title;
    BoundUIComponent Image_BackGround;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
    }

   
}