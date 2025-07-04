using DialogueSystem;
using UnityEngine;

namespace UI
{
    public class MenuUIState : UIState
    {
        public void Resume()
        {
            UIManager.Close();
        }

        public void ReturntoTitle()
        {
            DialogueManager.Clear();
        }
    }
}