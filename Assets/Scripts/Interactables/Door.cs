using Player;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    public class Door : Interactable
    {
        [Header("Setup")]
        public Transform hinge;

        [Header("Settings")]
        [Range(0, 180)]
        public float startAngle;
        [Range(0, 180)] 
        public float maxAngle = 120;

        [HideInInspector]
        public DoorState state;

        private PlayerController controller;
        private float _angle;

        private Vector3 _originalCameraPosition;
        private Quaternion _originalCameraRotation;
        private bool _returningCamera = false;

        public bool isInFront = false;

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

        private void Update()
        {
            if(_returningCamera)
                ReturnCamera();
            HandleRotation();
        }

        private void ReturnCamera()
        {
            Transform cam = controller.CameraTransform;
            cam.localPosition = Vector3.Lerp(cam.localPosition, _originalCameraPosition, Time.deltaTime * 10);
            cam.localRotation = Quaternion.Slerp(cam.localRotation, _originalCameraRotation, Time.deltaTime * 10);

            if(Vector3.Distance(cam.localPosition, _originalCameraPosition) < 0.05f)
            {
                cam.localPosition = _originalCameraPosition;
                cam.localRotation = _originalCameraRotation;
                controller.SwitchMouseStrategy<LookAround>();
                _returningCamera = false;
            }
        }

        private void HandleRotation()
        {
            const float speed = 5;
            float nextAngle = Mathf.Lerp(hinge.localEulerAngles.y, _angle, Time.deltaTime * speed);
            SetLocalDoorRotation(nextAngle);
        }

        private bool EstimateDesiredForwards(PlayerController controller)
        {
            Vector3 point = controller.CameraTransform.position;

            if (Vector3.Dot(point - hinge.position, hinge.forward) < 0)
                return true; //if the player stands in front of the door, assume he wants to close it

            if (Vector3.Dot(point - transform.position, transform.forward) > 0)
                return false; // if the player stands behind the doorpost, assume he wants to open it

            Vector3 localPoint = transform.TransformPoint(point);
            localPoint.z = 0;
            return localPoint.magnitude > 0.5f; //if neither, rely on how far away the player is from the door itself
        }

        public void HandleCamera(Transform cameraTransform)
        {
            Vector3 relPosition;
            Vector3 relDirection;


            if (isInFront)
            {
                relPosition = new Vector3(1.1f, 1.6f, Mathf.Lerp(-0.2f, 0.3f, hinge.localEulerAngles.y / 90));
                relDirection = Vector3.Lerp(new Vector3(15, -50, -15), new Vector3(-10, -90, 0), hinge.localEulerAngles.y / 180);
            }
            else
            {
                relPosition = Vector3.Lerp(new Vector3(0.5f, 1.6f, 0.3f), new Vector3(0.2f, 1.6f, 0.5f), hinge.localEulerAngles.y / 180);
                relDirection = Vector3.Lerp(new Vector3(0, 120, -20), new Vector3(0, 0, 0), hinge.localEulerAngles.y / 180);
            }

            Vector3 targetPos = hinge.TransformPoint(relPosition);
            Quaternion targetRot = Quaternion.Euler(relDirection + hinge.transform.eulerAngles);

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * 10);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRot, Time.deltaTime * 5);
        }

        /// <summary>
        /// Called when the player hits the interact key while targeting the door
        /// </summary>
        /// <param name="controller"></param>
        override public void OnUseStart (PlayerController controller)
        {
            _originalCameraPosition = controller.CameraTransform.localPosition;
            _originalCameraRotation = controller.CameraTransform.localRotation;

            this.controller = controller;
            controller.SwitchMouseStrategy<DoorMouseStrategy>();

            isInFront = EstimateDesiredForwards(controller);
        }

        public override void OnUse (PlayerController controller)
        {
            HandleCamera(controller.CameraTransform);
        }

        override public void OnUseStop (PlayerController controller)
        {
            _returningCamera = true;
        }

        public override void OnInteract(PlayerController controller) { }

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
