using UnityEngine;

namespace Effects
{
    public class CandleFlame : MonoBehaviour
    {
        [SerializeField] Renderer _flameRenderer;
        [SerializeField] float _windSmoothing = 5f;
        [SerializeField] float _windStrength = 1;

        Material _flameMaterial;
        Vector3 _previousPosition;
        Vector3 _velocitySmoothed;

        private void Start()
        {
            _flameMaterial = _flameRenderer.material;
            if (!_flameMaterial.HasVector("_WindVector"))
                Debug.LogWarning("Flame renderer doesnt have a WindVelocity field in the material, and will not have it set!", this);
        }

        void Update()
        {
            var delta = _flameRenderer.transform.position - _previousPosition;
            _previousPosition = _flameRenderer.transform.position;

            var deltaMag = delta.magnitude / Time.deltaTime;
            deltaMag = 1 - Mathf.Exp(-deltaMag);
            delta = delta.normalized * deltaMag * _windStrength;

            _velocitySmoothed -= (_velocitySmoothed - delta) * (1 - Mathf.Exp(-Time.deltaTime * _windSmoothing));
            _flameMaterial.SetVector("_WindVector", (Vector4)(-_velocitySmoothed));
        }
    }
}
