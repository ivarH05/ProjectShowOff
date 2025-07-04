using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    public void Toggle(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;
        UIManager.ToggleState<MenuUIState>();
    }
}
