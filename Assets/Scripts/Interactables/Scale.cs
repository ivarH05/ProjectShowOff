using Player;
using Player.InventoryManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


namespace Interactables
{
    public class Scale : MonoBehaviour
    {
        [SerializeField] private Transform beam;
        [SerializeField] private Placable basket1;
        [SerializeField] private Placable basket2;
        [SerializeField] private float angleLimit;

        public float weight1;

        public float weight2;

        public Events events;

        float _angularVelocity;

        float _angle;

        private void Start()
        {
            basket1.events.OnPlace.AddListener(OnPlace1);
            basket1.events.OnRemove.AddListener(OnRemove1);
            basket2.events.OnPlace.AddListener(OnPlace2);
            basket2.events.OnRemove.AddListener(OnRemove2);
        }

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
            basket1.transform.localEulerAngles = new Vector3(-_angle, 0, 0);
            basket2.transform.localEulerAngles = new Vector3(-_angle, 0, 0);

        }

        void OnPlace1(Placable placable, Item item, PlayerController player)
        {
            weight1 = item.weight;
            if (weight1 == weight2)
                events.OnBalance.Invoke();
        }
        void OnRemove1(Placable placable, Item item, PlayerController player)
        {
            weight1 = 0;
            if (weight1 == weight2)
                events.OnBalance.Invoke();
        }

        void OnPlace2(Placable placable, Item item, PlayerController player)
        {
            weight2 = item.weight;
            if (weight1 == weight2)
                events.OnBalance.Invoke();
        }
        void OnRemove2(Placable placable, Item item, PlayerController player)
        {
            weight2 = 0;
            if (weight1 == weight2)
                events.OnBalance.Invoke();
        }

        [System.Serializable]
        public struct Events
        {
            public UnityEvent OnBalance;
        }
    }
}
