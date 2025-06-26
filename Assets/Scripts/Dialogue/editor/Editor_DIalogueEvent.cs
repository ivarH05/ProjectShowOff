using DialogueSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(DialogueEvent))]
public class DialogueEventDrawer : PropertyDrawer
{
    private const float LabelWidth = 60f;
    private float VerticalSpacing = EditorGUIUtility.standardVerticalSpacing;
    private float FieldHeight = EditorGUIUtility.singleLineHeight;

    private int lines = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        lines = 0;
        float y = position.y;
        var typeProp = property.FindPropertyRelative("type");

        // Draw 'type' with custom label + field rects
        DrawLabeledField(position.x, ref y, "Type", typeProp, LabelWidth);

        switch((DialogueEventType)typeProp.enumValueIndex)
        {
            case DialogueEventType.SetConditionFlag:
                DrawLabeledField(position.x, ref y, "Flag", property.FindPropertyRelative("flag"), LabelWidth);
                DrawLabeledField(position.x, ref y, "value", property.FindPropertyRelative("flagValue"), LabelWidth);
                break;

            case DialogueEventType.PlaySound:
                DrawLabeledField(position.x, ref y, "Sound", property.FindPropertyRelative("sound"), LabelWidth);
                break;

            case DialogueEventType.ShakeCamera:
                DrawLabeledField(position.x, ref y, "Magnitude", property.FindPropertyRelative("shakeMagnitude"), LabelWidth);
                break;

            case DialogueEventType.SetSprite:
                DrawLabeledField(position.x, ref y, "Sprite", property.FindPropertyRelative("sprite"), LabelWidth);
                break;
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
        lines++;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return lines * (FieldHeight + VerticalSpacing);
    }
}
