using DialogueSystem;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueEvent))]
public class DialogueEventDrawer : PropertyDrawer
{
    private const float LabelWidth = 60f;
    private float VerticalSpacing = EditorGUIUtility.standardVerticalSpacing;
    private float FieldHeight = EditorGUIUtility.singleLineHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;

        var typeProp = property.FindPropertyRelative("type");
        var flagProp = property.FindPropertyRelative("flag");
        var boolProp = property.FindPropertyRelative("_boolean");

        // Draw 'type' with custom label + field rects
        DrawLabeledField(position.x, ref y, "Type", typeProp, LabelWidth);

        if ((DialogueEventType)typeProp.enumValueIndex == DialogueEventType.SetConditionFlag)
        {
            DrawLabeledField(position.x, ref y, "Flag", flagProp, LabelWidth);
            DrawLabeledField(position.x, ref y, "Value", boolProp, LabelWidth);
        }

        EditorGUI.EndProperty();
    }

    private void DrawLabeledField(float x, ref float y, string label, SerializedProperty property, float labelWidth)
    {
        var labelRect = new Rect(x, y, labelWidth, FieldHeight);
        var fieldRect = new Rect(x + labelWidth + 2f, y, 200 - labelWidth, FieldHeight);

        EditorGUI.LabelField(labelRect, label);
        EditorGUI.PropertyField(fieldRect, property, GUIContent.none);

        y += FieldHeight + VerticalSpacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var typeProp = property.FindPropertyRelative("type");
        int lines = 1;

        if ((DialogueEventType)typeProp.enumValueIndex == DialogueEventType.SetConditionFlag)
        {
            lines += 2;
        }

        return lines * (FieldHeight + VerticalSpacing);
    }
}
