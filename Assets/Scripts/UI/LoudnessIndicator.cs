using System.Collections;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class LoudnessIndicator : MonoBehaviour
    {
        [HideInInspector] public float Loudness;
        [SerializeField] float _updateSpeed = 1;
        [SerializeField] GameObject _barPrefab;
        [SerializeField] int _barCount;
        LoudnessIndicatorBar _first;

        private void Start()
        {
            if(!_barPrefab?.GetComponentInChildren<LoudnessIndicatorBar>())
            {
                Debug.LogError("LoudnessIndicator is missing a bar prefab with a loudness indicator bar component.", this);
                return;
            }

            LoudnessIndicatorBar prev = null;
            for (int i = 0; i < _barCount; i++)
            {
                GameObject instance = Instantiate(_barPrefab, transform);
                var bar = instance.GetComponent<LoudnessIndicatorBar>();
                if (i == 0)
                    _first = bar;
                
                if (prev != null)
                    prev.Next = bar;
                prev = bar;

                bar.MaxHeight = GetComponent<RectTransform>().sizeDelta.y;
            }

            StartCoroutine(UpdateLoudness());
        }

        IEnumerator UpdateLoudness()
        {
            while(true)
            {
                yield return new WaitForSeconds(_updateSpeed / _barCount);
                _first.SetLoudness(Loudness);
            }
        }
    }
}
