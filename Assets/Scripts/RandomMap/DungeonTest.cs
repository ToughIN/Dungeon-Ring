using System;
using UnityEngine;
using ToufFrame;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;

public class DungeonTest : MonoBehaviour
{
    DungeonGenerator dungeonGenerator;
    public int[,] dungeonMap;
    PlayerControls controls;
    public TriggerVariable<bool> ifGenerate = new TriggerVariable<bool>(false);

    private void Awake()
    {
        controls = new PlayerControls(); // 初始化PlayerControls
        controls.Test._1.performed += ctx => Test1();
        controls.Test._2.performed += ctx => Test2();
        controls.Test._3.performed += ctx => Test3();
        controls.Test._4.performed += ctx => Test4();
        controls.Test._5.performed += ctx => Test5();
    }

    private void OnEnable()
    {
        controls.Enable(); // 启用输入控制
    }

    public void Test1()
    {
        UIMgr.Instance.ShowPanel<UI_Panel_Inventory>(GameStrings.ADDRESSABLES_UI_CANVAS_PlayerHUD,GameStrings.ADDRESSABLES_UI_Panel_Inventory,EUIlayer.Top);
    }

    public void Test2()
    {
        UIMgr.Instance.ClosePanel(GameStrings.ADDRESSABLES_UI_Panel_Inventory);
    }

    public void Test3()
    {
        
    }

    public void Test4()
    {
        
    }

    public void Test5()
    {
        
    }

    private void OnDisable()
    {
        controls.Disable(); // 禁用输入控制
    }

    private void Update()
    {
        
    }
}