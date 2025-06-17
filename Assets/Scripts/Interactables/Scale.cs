using UnityEngine;
using UnityEngine.UIElements;


namespace Interactables
{
    public class Scale : MonoBehaviour
    {
        [SerializeField] private Transform beam;
        [SerializeField] private Transform basket1;
        [SerializeField] private Transform basket2;
        [SerializeField] private float angleLimit;

        public float weight1;

        public float weight2;

        float _angularVelocity;

        float _angle;

        // Update is called once per frame
        void FixedUpdate()
        {
            float WeightDifference = weight1 - weight2;
            _angularVelocity += WeightDifference * Time.fixedDeltaTime * 981f - _angle * 0.2f;
            _angularVelocity = Mathf.Lerp(_angularVelocity, 0, Time.fixedDeltaTime);

            _angle += _angularVelocity * Time.fixedDeltaTime;

            if (_angle < -angleLimit)
            {
                _angle = -angleLimit;
                _angularVelocity = -0.5f * _angularVelocity;
            }
            if (_angle > angleLimit)
            {
                _angle = angleLimit;
                _angularVelocity = -0.5f * _angularVelocity;
            }
            _angle = Mathf.Clamp(_angle, -angleLimit, angleLimit);

            beam.transform.localEulerAngles = new Vector3(_angle, 0, 0);
            basket1.localEulerAngles = new Vector3(-_angle, 0, 0);
            basket2.localEulerAngles = new Vector3(-_angle, 0, 0);

        }
    }
}
