using UnityEngine;

namespace UI
{
    public class TryExitCryptUIState : UIState
    {
        [SerializeField] CanvasGroupFader _canvas;

        public void Close()
        {
            UIManager.Close();
        }

        public override void OnStateStart()
        {
            base.OnStateStart();
            _canvas.FadeIn();
        }
        public override void OnStateStop()
        {
            base.OnStateStop();
            _canvas.FadeOut();
        }
    }
}
