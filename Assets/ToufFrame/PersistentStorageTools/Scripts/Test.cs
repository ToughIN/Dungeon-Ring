using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ToufFrame
{
    public class Test : MonoBehaviour
    {
        private void Start()
        {
            BinaryDataMgr.Instance.InitData();


            
            
            
            
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                PlayerInfoContainer data = BinaryDataMgr.Instance.GetTable<PlayerInfoContainer>();
                
                print(data.dataDic[2].name);


                PlayerInfo testPlayerInfo = new PlayerInfo();
                testPlayerInfo = data.dataDic[3];
                print("测试存档:"+testPlayerInfo.name);
                testPlayerInfo.name += "因为存档修改了";
                BinaryDataMgr.Instance.Save(testPlayerInfo,"test");

                PlayerInfo testPlayerInfo2 = BinaryDataMgr.Instance.Load<PlayerInfo>("test");
                print("测试读档:"+testPlayerInfo2.name);
            }
        }
    }
}