using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;

[InitializeOnLoad]
public static class EditorMouse
{
    static bool isLocked = false; // Set this from your script
    static GameObject _target;

    static EditorMouse()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (!isLocked) return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (Selection.activeGameObject != _target)
        {
            Selection.activeGameObject = _target;
        }
    }

    // Call this from your script to toggle edit mode
    public static void Lock(GameObject target)
    {
        isLocked = true;
        _target = target;
    }

    public static void Unluck()
    {
        isLocked = false;
        _target = null;
    }
}

