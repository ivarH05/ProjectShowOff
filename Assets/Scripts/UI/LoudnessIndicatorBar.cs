using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class LoudnessIndicatorBar : MonoBehaviour
    {
        [HideInInspector] public LoudnessIndicatorBar Next;
        [HideInInspector] public float MaxHeight;
        [SerializeField] float _randomStrength = 1;
        float _currentLoudness;
        float _minHeight;
        float _avgDiff;
        RectTransform _image;

        private void Start()
        {
            _image = GetComponent<RectTransform>();
            _minHeight = _image.sizeDelta.x;
            _image.sizeDelta = new Vector2(_minHeight, _minHeight);
        }

        public void SetLoudness(float value)
        {
            Next?.SetLoudness(_currentLoudness);
            float diff = Mathf.Abs(_currentLoudness - value);
            _currentLoudness = value;
            
            _avgDiff = .8f * _avgDiff + .2f * diff;
            var cur = _image.sizeDelta;
            cur.y = Mathf.Clamp((_currentLoudness + Random.Range(-_avgDiff, _avgDiff) * _randomStrength * .5f) * MaxHeight, _minHeight, MaxHeight);
            _image.sizeDelta = cur;
        }
    }
}
