using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    [RequireComponent(typeof(CanvasGroup), typeof(Canvas))]
    public class SceneFader : MonoBehaviour
    {
        [SerializeField] float _fadeoutTimeSeconds = 1f;

        static SceneFader _instance;

        CanvasGroup _group;
        AsyncOperation _sceneLoad;
        float _fadeTimer = -1;
        bool _transitionDone;
        bool _isProxy;

        private void Start()
        {
            if(_instance != null)
            {
                _isProxy = true;
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            _group = GetComponent<CanvasGroup>();
            if (Time.timeSinceLevelLoad <= _fadeoutTimeSeconds)
            {
                _group.alpha = 1;
            }
        }

        public void TransitionScene(string targetScene)
        {
            if (_isProxy)
            {
                _instance.TransitionScene(targetScene);
                return;
            }

            if (_sceneLoad != null) return;
            try{
                _sceneLoad = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
            }
            catch (System.Exception e) {
                Debug.LogError($"Attempted to load {targetScene}, but something went wrong: {e}");
                return;
            }

            _sceneLoad.allowSceneActivation = false;
            _group.blocksRaycasts = true;
            _group.interactable = true;
            Time.timeScale = 0;
            _fadeTimer = 0;
        }

        private void Update()
        {
            if (_isProxy) return;

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
                _sceneLoad.allowSceneActivation = true;
                _sceneLoad = null;
                Time.timeScale = 1;
                _group.blocksRaycasts = false;
                _group.interactable = false;
                _group.alpha = 1;
                return;
            }
            if (progress > 2)
            {
                _transitionDone = false;
                _group.alpha = 0;
                _fadeTimer = -1;
                return;
            }
            if (progress > 1)
                progress = 2 - progress;
            _group.alpha = progress;
        }
    }
}
