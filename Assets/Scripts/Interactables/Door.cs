using Player;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    public class Door : MonoBehaviour
    {
        [Header("Setup")]
        public Transform hinge;

        [Header("Debug setup")]
        public PlayerController controller;

        [Header("Settings")]
        [Range(0, 180)]
        public float startAngle;
        [Range(0, 180)] 
        public float maxAngle = 120;

        private float _angle;

        private void OnDrawGizmos()
        {
            _angle = startAngle;
            DrawDoorGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (maxAngle < 0 || maxAngle > 180)
                Gizmos.color = Color.red;

            SetLocalDoorRotation(startAngle);
            DrawDoorGizmos();
        }

        private void Start()
        {
            OnInteract(controller);
        }

        private void FixedUpdate()
        {
            const float speed = 5;
            float nextAngle = Mathf.Lerp(hinge.localEulerAngles.y, _angle, Time.fixedDeltaTime * speed);
            SetLocalDoorRotation(nextAngle);
        }

        void OnInteract(PlayerController controller)
        {
            controller.SwitchStrategy(new DoorMouseStrategy(this));
        }

        public void Move(float deltaAngle)
        {
            _angle += deltaAngle;
            if(_angle < 0)
                _angle = 0;
            if(_angle > maxAngle)
                _angle = maxAngle;
        }

        private void SetLocalDoorRotation(float newAngle)
        {
            hinge.localEulerAngles = new Vector3(
                hinge.localEulerAngles.x,
                newAngle,
                hinge.localEulerAngles.z);
        }

        private void DrawDoorGizmos()
        {
            if (startAngle > maxAngle || _angle > maxAngle)
                Gizmos.color = Color.red;

            const float radius = 0.8f;

            List<Vector3> points = new List<Vector3>() { Vector3.zero };

            for (float a = -maxAngle; a < 0; a += 10)
            {
                float rads = a * Mathf.Deg2Rad;
                float x = Mathf.Cos(rads) * radius;
                float z = Mathf.Sin(rads) * radius;

                points.Add(new Vector3(x, 0f, z));
            }

            points.Add(new Vector3(radius, 0f, 0));

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawLineStrip(points.ToArray(), true);
        }
    }
}
