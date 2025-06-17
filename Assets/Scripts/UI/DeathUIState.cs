using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DeathUIState : UIState
    {
        [SerializeField] private CanvasGroup _canvas;
        public void Start()
        {
            _canvas = GetComponent<CanvasGroup>();
        }
        public override void OnStateStart()
        {
            base.OnStateStart();
            _canvas.alpha = 0;
        }

        private void Update()
        {
            _canvas.alpha = Mathf.Lerp(_canvas.alpha, 1, Time.deltaTime * 3);
        }
    }
}