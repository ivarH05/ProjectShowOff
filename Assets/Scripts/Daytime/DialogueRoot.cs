using System;
using System.Collections;
using UnityEngine;


namespace Daytime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DialogueRoot : MonoBehaviour
    {
        public static DialogueRoot Instance {get; private set;}

        public event Action OnDialogueEnable;
        public event Action OnDialogueDisable;

        [SerializeField] float FadeAnimationLength = 1; 

        CanvasGroup _group;

        private void Awake()
        {
            if (Instance != null) Debug.LogError("There was already a DialogueRoot in the scene! This one may not work correctly.", this);
            else Instance = this;
        }

        private void OnEnable()
        {
            _group = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Enables the dialogue.
        /// </summary>
        public void Enable()
        {
            _group.interactable = true;
            _group.blocksRaycasts = true;
            StartCoroutine(TransparencyAnimation(false, 1/FadeAnimationLength));
            OnDialogueEnable?.Invoke();
        }

        /// <summary>
        /// Disables the dialogue.
        /// </summary>
        public void Disable()
        {
            _group.interactable = false;
            _group.blocksRaycasts = false;
            StartCoroutine(TransparencyAnimation(true, 1/FadeAnimationLength));
            OnDialogueDisable?.Invoke();
        }

        IEnumerator TransparencyAnimation(bool makeHidden, float fadeSpeed)
        {
            float pAlpha = _group.alpha;
            while(_group.alpha == pAlpha && (makeHidden ? pAlpha > 0 : pAlpha < 1))
            {
                pAlpha += Time.deltaTime * fadeSpeed * (makeHidden ? -1 : 1);
                _group.alpha = pAlpha;
                yield return null;
            }
            if(_group.alpha == pAlpha)
            {
                _group.alpha = makeHidden ? 0 : 1;
            }
        }
    }
}
