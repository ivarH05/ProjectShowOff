using UnityEditor;
using UnityEngine;

namespace UI
{
    [InitializeOnLoad]
    public static class UIStateSelector
    {
        static UIState currentState;

        static UIStateSelector()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        static void OnSelectionChanged()
        {
            currentState?.gameObject.SetActive(false);

            var selected = Selection.activeGameObject;

            if (selected == null)
                return;

            UIState newState = selected.GetComponent<UIState>();
            if (newState == null)
                newState = selected.GetComponentInParent<UIState>(true);

            if (newState == null)
                return;

            SwitchState(newState);
        }

        static void SwitchState(UIState newState)
        {
            newState.gameObject.SetActive(true);

            currentState = newState;
        }
    }
}