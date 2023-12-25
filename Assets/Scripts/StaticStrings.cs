using System.Threading;
using UnityEngine;

public static class GameStrings
{
    #region AnimationRelated

    public static readonly string ANIMATION_Swap_Right_Weapon_01 = "Swap_Right_Weapon_01";
    public static readonly string ANIMATION_Swap_Left_Weapon_01 = "Swap_Left_Weapon_01";
    
    
    public static readonly string ANIMATION_MAIN_LIGHT_ATTACK_01 = "Main_Light_Attack_01";
    public static readonly string ANIMATION_MAIN_LIGHT_ATTACK_02 = "Main_Light_Attack_02";
    public static readonly string ANIMATION_MAIN_HEAVY_ATTACK_01 = "Main_Heavy_Attack_01";

    public static readonly string ANIMATION_MAIN_JUMP_START_01 = "Main_Jump_Start_01";
    
    
    public static readonly string ANIMATION_HIT_FORWARD_MEDIUM_01 = "Hit_Forward_Medium_01";
    public static readonly string ANIMATION_HIT_BACKWARD_MEDIUM_01 = "Hit_Backward_Medium_01";
    public static readonly string ANIMATION_HIT_LEFT_MEDIUM_01 = "Hit_Left_Medium_01";
    public static readonly string ANIMATION_HIT_RIGHT_MEDIUM_01 = "Hit_Right_Medium_01";
    
    public static readonly string ANIMATION_HIT_FORWARD_MEDIUM_02 = "Hit_Forward_Medium_02";
    public static readonly string ANIMATION_HIT_BACKWARD_MEDIUM_02 = "Hit_Backward_Medium_02";
    public static readonly string ANIMATION_HIT_LEFT_MEDIUM_02 = "Hit_Left_Medium_02";
    public static readonly string ANIMATION_HIT_RIGHT_MEDIUM_02 = "Hit_Right_Medium_02";

    public static readonly string ANIMATION_ROLL_FORWARD_01 = "Roll_Forward_01";
    
    
    //动画参数名
    public static readonly string VARIABLE_ANIMATOR_IsChargingAttack = "IsChargingAttack";
    public static readonly string VARIABLE_ANITAMOR_IsGrounded = "IsGrounded";
    public static readonly string VARIABLE_ANIMATOR_InAirTimer = "InAirTimer";

    #endregion
    
    
    #region Addressables
    //Construct
    public static readonly string ADDRESSABLES_ROOM_Prefab = "Room";
    public static readonly string ADDRESSABLES_ROOM_Corridor = "Corridor";
    
    //UI_Main
    public static readonly string ADDRESSABLES_UI_CANVAS_MainMenu = "Main_Canvas";
    public static readonly string ADDRESSABLES_UI_PANEL_Buttons = "UI_Panel_MainMenu_Buttons";
    public static readonly string ADDRESSABLES_UI_PANEL_LoadMenu = "UI_Panel_LoadMenu";
    public static readonly string ADDRESSABLES_UI_PANEL_Background = "UI_Panel_MainMenu_BackGround";
    
    //UI_HUD
    public static readonly string ADDRESSABLES_UI_CANVAS_PlayerHUD = "Player_HUD_Canvas";
    public static readonly string ADDRESSABLES_UI_PANEL_HUD_Basic = "UI_Panel_HUD_Basic";
    
    //UI_Inventory
    public static readonly string ADDRESSABLES_UI_Panel_Inventory = "UI_Panel_HUD_Inventory";
    public static readonly string ADDRESSABLES_UI_CONTROL_InventorySlot = "Slot_Inventory";
    
    //ScriptableObject Items
    public static readonly string ADDRESSABLES_SO_ITEM_Weapon_ShortSowrd = "SO_ShortSword";
    
    
    
    #endregion
    
    //Path
    public static readonly string PATH_SAVE_FILE_DIRECTORY = Application.persistentDataPath + "/SaveFiles/";

}