using UnityEngine;

namespace Crypts
{
    public class TimeHandler : MonoBehaviour
    {
        [SerializeField] float _maxSecondsInCrypt = 100;
        public float Progress01 => Time.timeSinceLevelLoad / _maxSecondsInCrypt;

        private void Update()
        {

        }
    }
}
