using System.Collections;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        [SerializeField] float _fadeSpeedSeconds = 1;
        [SerializeField] bool _startVisible;
        CanvasGroup _canvas;
        private void OnEnable()
        {
            _canvas = GetComponent<CanvasGroup>();
            _canvas.interactable = _startVisible;
            _canvas.blocksRaycasts = _startVisible;
            if(_startVisible)
                _canvas.alpha = 1;
            else _canvas.alpha = 0;
        }

        public void FadeIn()
        {
            _canvas.interactable = true;
            _canvas.blocksRaycasts = true;
            StartCoroutine(FadeCanvas(1));
        }

        public void FadeOut() 
        {
            _canvas.interactable = false;
            _canvas.blocksRaycasts = false;
            StartCoroutine(FadeCanvas(0));
        }

        IEnumerator FadeCanvas(float value)
        {
            float start = _canvas.alpha;
            float cur = _canvas.alpha;
            float time = 0;
            while (cur == _canvas.alpha)
            {
                time += Time.deltaTime;
                _canvas.alpha = Mathf.Lerp(start, value, time / _fadeSpeedSeconds);
                if (_canvas.alpha <= 0)
                {
                    _canvas.alpha = 0;
                    break;
                }
                cur = _canvas.alpha;
                yield return null;
            }
        }
    }
}
