using ToufFrame;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TriggerVariable<>))]
public class TriggerVariableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 获取对_value的引用
        SerializedProperty valueProperty = property.FindPropertyRelative("_value");

        // 绘制字段
        EditorGUI.PropertyField(position, valueProperty, label, true);

        EditorGUI.EndProperty();
    }
}