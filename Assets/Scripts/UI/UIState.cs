using UI;
using UnityEngine;

namespace UI
{
    public abstract class UIState : MonoBehaviour
    {
        [SerializeField] private bool _unlockCursor = true;
        private CursorLockMode lastCursor;

        public virtual void OnStateStart()
        {
            gameObject.SetActive(true);

            lastCursor = Cursor.lockState;

            if (_unlockCursor)
                Cursor.lockState = CursorLockMode.None;
        }

        public virtual void OnStateStop()
        {
            gameObject.SetActive(false);

            if (_unlockCursor)
                Cursor.lockState = lastCursor;
        }

        public virtual void OnEvent<T>(T context) where T : UIEvent { }
    }
}