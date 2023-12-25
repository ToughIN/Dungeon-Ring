using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Reflection;
using UnityEngine.EventSystems;

namespace ToufFrame
{
    [CustomEditor(typeof(BasePanel),true)]
    public class BasePanelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BasePanel panel = (BasePanel)target;
            if (GUILayout.Button("Generate UI Code"))
            {
                GenerateUICode(panel);
            }
            // if(GUILayout.Button("Assign UI References"))
            // {
            //     
            // }
        }

        private void GenerateUICode(BasePanel panel)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using ToufFrame;");
            sb.AppendLine("using TMPro;"); // 如果你使用 TextMeshPro
            sb.AppendLine("");
            sb.AppendLine("public partial class " + panel.GetType().Name+" : BasePanel");
            sb.AppendLine("{");

            BoundUIComponent[] boundComponents = panel.GetComponentsInChildren<BoundUIComponent>();
            foreach (var boundComponent in boundComponents)
            {
                boundComponent.GetContrls();
                
                //存到一个容器中，能否用回调的方式，将这些组件传递到BasePanel中
                
                string typeName = "BoundUIComponent";
                string varName = "BUC_"+boundComponent.gameObject.name;
                sb.AppendLine($"    public {typeName} {varName};");
            }

            sb.AppendLine("}");

            string folderPath = Path.Combine(Application.dataPath, "GeneratedUI"); // 修改为你希望保存文件的位置
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, panel.GetType().Name + ".Generated.cs");
            File.WriteAllText(filePath, sb.ToString());

            AssetDatabase.Refresh();
            EditorApplication.delayCall+=()=>BindComponentsToPanel(panel, boundComponents);
        }
        
        
        private void BindComponentsToPanel(BasePanel panel, BoundUIComponent[] boundComponents)
        {
            // 确保编译完成
            if (EditorApplication.isCompiling)
            {
                EditorApplication.delayCall += () => BindComponentsToPanel(panel, boundComponents);
                return;
            }

            // 使用传递的 BoundUIComponent 实例
            foreach (var boundComponent in boundComponents)
            {
                string fieldName = "BUC_" + boundComponent.gameObject.name;
                FieldInfo fieldInfo = panel.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(panel, boundComponent);
                }
            }

            // 更新 Inspector
            EditorUtility.SetDirty(panel);
        }


    }

}