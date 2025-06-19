using UnityEngine;

namespace Crypts
{
    public class TimeHandler : MonoBehaviour
    {
        [SerializeField] Transform _timerTransform;
        [SerializeField] float _maxSecondsInCrypt = 100;
        public float Progress01 => Time.timeSinceLevelLoad / _maxSecondsInCrypt;

        private void Update()
        {
            if (_timerTransform == null) return;

            _timerTransform.rotation = Quaternion.AngleAxis(Progress01*-360, Vector3.forward);
        }
    }
}
