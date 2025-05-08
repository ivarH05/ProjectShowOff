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

        [HideInInspector]
        public DoorState state;

        private float _angle;

        /// <summary>
        /// check if door is closed or not
        /// </summary>
        public bool IsClosed
        {
            get { return _angle < 1; }
        }

        /// <summary>
        /// check if the door is fully locked, also usable to lock and unlock it
        /// </summary>
        public bool IsLocked
        {
            get 
            { 
                return state == DoorState.locked; 
            }

            set
            {
                if (value)
                {
                    state = DoorState.locked;
                    _angle = 0;
                }
                else if (IsClosed)
                    state = DoorState.closed;
                else
                    state = DoorState.open;
            }
        }


        ////////// Standard behaviour
        private void Start()
        {
            OnInteractStart(controller);
        }
        private void Update()
        {
            HandleCamera(controller.CameraTransform);
            HandleRotation();
        }

        private void HandleRotation()
        {
            const float speed = 5;
            float nextAngle = Mathf.Lerp(hinge.localEulerAngles.y, _angle, Time.deltaTime * speed);
            SetLocalDoorRotation(nextAngle);
        }

        public void HandleCamera(Transform cameraTransform)
        {
            Vector3 relPosition = new Vector3(1.1f, 1.6f, Mathf.Lerp(-0.2f, 0.3f, hinge.localEulerAngles.y / 90));
            Vector3 relRotation = Vector3.Lerp(new Vector3(15, -50, -15), new Vector3(-10, 50, 30), hinge.localEulerAngles.y / 180);

            Vector3 targetPos = hinge.TransformPoint(relPosition);
            Quaternion targetRot = Quaternion.Euler(hinge.TransformDirection(relRotation));

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * 5);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRot, Time.deltaTime * 3);
        }

        public bool IsInFront(Vector3 point)
        {
            return Vector3.Dot(point - transform.position, transform.position) < 0;
        }

        /// <summary>
        /// Called when the player hits the interact key while targeting the door
        /// </summary>
        /// <param name="controller"></param>
        void OnInteractStart(PlayerController controller)
        {
            this.controller = controller;
            controller.SwitchStrategy(new DoorMouseStrategy(this));
        }

        void OnInteractStop(PlayerController controller)
        {
            controller.SwitchStrategy(new LookAround());
        }

        /// <summary>
        /// Smoothy move the door by changing the target angle relative to the current angle
        /// </summary>
        /// <param name="deltaAngle"> the relative angle the door should rotate </param>
        public void Move(float deltaAngle)
        {
            _angle += deltaAngle;
            if(_angle < 0)
                _angle = 0;
            if(_angle > maxAngle)
                _angle = maxAngle;
        }

        /// <summary>
        /// Snap the door to angle
        /// </summary>
        /// <param name="newAngle">the target angle</param>
        private void SetLocalDoorRotation(float newAngle)
        {
            hinge.localEulerAngles = new Vector3(
                hinge.localEulerAngles.x,
                newAngle,
                hinge.localEulerAngles.z);
        }




        /////////// Designer Layout
        
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

        /// <summary>
        /// Draw the angle the door can rotate
        /// </summary>
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
