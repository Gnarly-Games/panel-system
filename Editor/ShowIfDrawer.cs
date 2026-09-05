using Gnarly.PanelSystem;
using UnityEditor;
using UnityEngine;
namespace panel_system.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            // Gizlendiğinde dikey boşluk kalmaması için yükseklik 0 verilir
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool ShouldShow(SerializedProperty property)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            // Aynı seviyedeki şart değişkenini arar (struct ve nested class'larda da çalışır)
            SerializedProperty conditionProp = property.serializedObject.FindProperty(
                GetRelativePropertyPath(property, showIf.ConditionFieldName)
            );

            if (conditionProp == null)
            {
                Debug.LogWarning($"[ShowIf] Şart değişkeni bulunamadı: {showIf.ConditionFieldName}");
                return true; // Hata durumunda alanı gizleyip erişimi engellememek için gösterilir
            }

            if (conditionProp.propertyType == SerializedPropertyType.Boolean)
            {
                return conditionProp.boolValue;
            }

            Debug.LogWarning($"[ShowIf] {showIf.ConditionFieldName} değişkeni bir bool değil!");
            return true;
        }

        private string GetRelativePropertyPath(SerializedProperty property, string targetFieldName)
        {
            string path = property.propertyPath;
            int lastDotIndex = path.LastIndexOf('.');

            if (lastDotIndex != -1)
            {
                return path.Substring(0, lastDotIndex + 1) + targetFieldName;
            }

            return targetFieldName;
        }
    }
}