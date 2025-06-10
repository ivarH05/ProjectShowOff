using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneFader : MonoBehaviour
    {
        [SerializeField] float _fadeoutTimeSeconds = 1f;
        [SerializeField] string _targetScene;

        CanvasGroup _group;
        float _fadeTimer = -1;
        bool _transitionDone;

        private void Start()
        {
            _group = GetComponent<CanvasGroup>();
            if (Time.timeSinceLevelLoad <= _fadeoutTimeSeconds)
            {
                _group.alpha = 1;
            }
        }

        public void TransitionScene()
        {
            if (_fadeTimer >= 0) return;

            _group.blocksRaycasts = true;
            Time.timeScale = 0;
            _fadeTimer = 0;
        }

        private void Update()
        {
            if (_fadeTimer < 0)
            {
                if (Time.timeSinceLevelLoad > _fadeoutTimeSeconds)
                    return;

                float pogress = Time.timeSinceLevelLoad / _fadeoutTimeSeconds;
                _group.alpha = 1 - pogress;
                return;
            }

            _fadeTimer += Time.unscaledDeltaTime;
            
            float progress = _fadeTimer / _fadeoutTimeSeconds;
            if(progress > 1 && !_transitionDone)
            {
                _transitionDone = true;
                SceneManager.LoadScene(_targetScene);
                Time.timeScale = 1;
            }
            _group.alpha = progress;
        }
    }
}
