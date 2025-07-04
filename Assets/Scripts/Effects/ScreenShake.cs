using UnityEngine;

namespace Effects
{
    public class ScreenShake : MonoBehaviour
    {
        public float Strength;
        public float MaxLocationChange = .01f;
        public float MaxRotationChange = 5f;
        [SerializeField] float _smoothness = 5f;

        Vector3 _targetPos;
        Quaternion _targetRot;

        void Update()
        {
            float interp = 1 - Mathf.Exp(-Time.deltaTime * _smoothness);
            transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, _targetPos, interp);
            transform.localRotation = Quaternion.LerpUnclamped(transform.localRotation, _targetRot, interp);
        }

        private void FixedUpdate()
        {
            _targetPos = Random.insideUnitSphere * MaxLocationChange * Strength;
            _targetRot = Quaternion.Euler(Random.value * MaxRotationChange *  Strength, Random.value * MaxRotationChange * Strength, Random.value * MaxRotationChange * Strength);
        }
    }
}
