using UnityEngine;

namespace Effects
{
    public class ScreenShake : MonoBehaviour
    {
        public float Strength;
        public float MaxLocationChange = .01f;
        public float MaxRotationChange = 5f;

        private void FixedUpdate()
        {
            transform.localPosition = Random.insideUnitSphere * MaxLocationChange * Strength;
            transform.localRotation = Quaternion.Euler(Random.value * MaxRotationChange *  Strength, Random.value * MaxRotationChange * Strength, Random.value * MaxRotationChange * Strength);
        }
    }
}
