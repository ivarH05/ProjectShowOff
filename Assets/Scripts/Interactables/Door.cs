using GameManagement;
using Player;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

namespace Interactables
{
    [RequireComponent(typeof(NavMeshObstacle))]
    public class Door : Interactable
    {
        [Header("Setup")]
        public Transform hinge;

        [Header("Settings")]
        [Range(0, 180)]
        public float startAngle;
        [Range(0, 180)] 
        public float maxAngle = 120;

        public bool flipped = false;

        [Space]
        public Events events = new Events();

        [SerializeField]
        private DoorState state;
        [HideInInspector]
        public bool isInFront = false;

        private bool _returningCamera = false;
        private float _angle;
        private float _currentAngle;

        private PlayerController controller;
        private Vector3 _originalCameraPosition;
        private NavMeshObstacle _obstacle;


        const float DoorSwingSpeed = 5;

        [System.Serializable]
        public class Events
        {
            public UnityEvent<Door, PlayerController> OnOpen;
            public UnityEvent<Door, PlayerController> OnMove;
            public UnityEvent<Door, PlayerController> OnClose;

            public UnityEvent<Door, PlayerController> OnLock;
            public UnityEvent<Door, PlayerController> OnUnlock;
        }

        /// <summary>
        /// The angle of the door.
        /// </summary>
        public float CurrentAngle => _angle;

        /// <summary>
        /// check if door is closed or not
        /// </summary>
        public bool IsClosed
        {
            get { return _angle < 1; }
        }

        private int SwingDirection { get => flipped ? -1 : 1; }

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
                if(_obstacle != null) 
                    _obstacle.enabled = value;
                if (value)
                {
                    if(state != DoorState.locked)
                        events.OnLock.Invoke(this, controller);
                    state = DoorState.locked;

                    Close();
                }
                else
                {
                    if (state == DoorState.locked)
                        events.OnUnlock.Invoke(this, controller);

                    if (IsClosed)
                        state = DoorState.closed;
                    else
                        state = DoorState.open;
                }
            }
        }


        ////////// Standard behaviour

        private void Start()
        {
            _angle = startAngle;
            _obstacle = GetComponent<NavMeshObstacle>();
            _obstacle.enabled = IsLocked;
        }

        private void Update()
        {
            if(_returningCamera)
                ReturnCamera();
            HandleRotation();
        }

        private float _tempAngle;
        private void OnTriggerEnter(Collider other)
        {
            if (IsLocked)
                return;

            if (!other.CompareTag("Enemy"))
                return;

            _tempAngle = _angle;

            for (int i = 0; i < PlayerManager.PlayerCount; i++)
            {
                PlayerController player = PlayerManager.GetPlayer(i);
                if (player.ActiveInteractable == this)
                    player.StopInteraction();
            }
            

            SetAngle(maxAngle);
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsLocked)
                return;

            if (!other.CompareTag("Enemy"))
                return;

            SetAngle(_tempAngle);
        }

        private void ReturnCamera()
        {
            if(controller == null)
                return;

            Transform cam = controller.CameraTransform;

            Quaternion targetRot = Quaternion.Euler(cam.localEulerAngles.x, cam.localEulerAngles.y, 0);
            cam.localRotation = Quaternion.Lerp(cam.localRotation, targetRot, Mathf.Clamp(Time.deltaTime * 40, 0, 0.8f));

            if (Mathf.Abs(cam.localEulerAngles.z) < 1)
            {
                Vector3 temp = cam.position;
                controller.Body.isKinematic = false;
                controller.MovePosition(controller.Body.position + (controller.transform.TransformPoint(cam.localPosition) - _originalCameraPosition));

                cam.position = temp;
                cam.localEulerAngles = new Vector3(cam.localEulerAngles.x, cam.localEulerAngles.y, 0);

                controller.SwitchMouseStrategy<LookAround>();
                controller.SwitchMovementStrategy<Walk>();
                controller.SwitchInteractStrategy<StandardInteract>();
                controller = null;
                _returningCamera = false;
                isUsing = false;
            }
        }

        private void HandleRotation()
        {
            float nextAngle = Mathf.Lerp(_currentAngle, _angle, Time.deltaTime * DoorSwingSpeed);
            SetLocalDoorRotation(nextAngle);
        }

        private bool EstimateDesiredForwards(PlayerController controller)
        {
            Vector3 point = controller.CameraTransform.position;

            if (Vector3.Dot(point - hinge.position, hinge.forward) < 0)
                return true; //if the player stands in front of the door, assume he wants to close it

            if (Vector3.Dot(point - transform.position, transform.forward) > 0)
                return false; // if the player stands behind the doorpost, assume he wants to open it

            Vector3 localPoint = point - transform.position;
            localPoint.y = 0;
            return localPoint.magnitude > 1; //if neither, rely on how far away the player is from the door itself
        }

        public void HandleCamera(Transform cameraTransform)
        {
            Vector3 relPosition;
            Vector3 relDirection;
            GetCameraTransforms(out relPosition, out relDirection);
            if (flipped)
            {
                relPosition.z *= -1;
                relDirection.y *= -1;
                relDirection.y += 180;
                relDirection.z *= -1;
            }

            Vector3 targetPos = hinge.TransformPoint(relPosition);
            Quaternion targetRot = Quaternion.Euler(relDirection + hinge.transform.eulerAngles);

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * 10);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRot, Time.deltaTime * 5);
        }

        /// <summary>
        /// Get the relative position of the camere when peeking
        /// </summary>
        /// <param name="PositionResult">the relative position</param>
        /// <param name="DirectionResult">the relative rotation</param>
        public virtual void GetCameraTransforms(out Vector3 PositionResult, out Vector3 DirectionResult)
        {
            if (isInFront != flipped)
            {
                PositionResult = new Vector3(1.1f, 1.6f, Mathf.Lerp(-0.2f, 0.3f, _currentAngle / 90));
                DirectionResult = Vector3.Lerp(new Vector3(15, -50, -15), new Vector3(-10, -90, 0), _currentAngle / 180);
            }
            else
            {
                PositionResult = Vector3.Lerp(new Vector3(0.5f, 1.6f, 0.3f), new Vector3(0.2f, 1.6f, 0.5f), _currentAngle / 180);
                DirectionResult = Vector3.Lerp(new Vector3(0, 120, -20), new Vector3(0, 0, 0), _currentAngle / 180);
            }
        }

        bool isUsing = false;

        override public void OnUseStart (PlayerController controller)
        {
            if (IsLocked || isUsing)
                return;
            isUsing = true;
            _originalCameraPosition = controller.transform.TransformPoint(controller.CameraTransform.localPosition);

            this.controller = controller;
            controller.SwitchMouseStrategy<DoorMouseStrategy>();
            controller.DisableMovement();
            controller.Body.isKinematic = true;

            isInFront = EstimateDesiredForwards(controller);

            if (IsClosed)
            {
                events.OnOpen.Invoke(this, controller);
                SetAngle(15);
            }
            else
                SetAngle(50);
        }

        public override void OnUse (PlayerController controller)
        {
            if (IsLocked || !isUsing)
                return;
            HandleCamera(controller.CameraTransform);
        }

        override public void OnUseStop (PlayerController controller)
        {
            if (IsLocked || !isUsing)
                return;

            if (_angle < 10)
                Close();
            else
                SetAngle(maxAngle);

            _returningCamera = true;
            controller.ClearInteractStrategy();
        }

        /// <summary>
        /// Close the door
        /// </summary>
        private void Close()
        {
            SetAngle(0);
            events.OnClose.Invoke(this, controller);
        }

        public override void OnInteract(PlayerController controller) { }

        /// <summary>
        /// Smoothy move the door by changing the target angle relative to the current angle
        /// </summary>
        /// <param name="deltaAngle"> the relative angle the door should rotate </param>
        public void Move(float deltaAngle)
        {
            SetAngle(_angle + deltaAngle);
        }

        public void SetAngle(float angle)
        {
            if (IsLocked)
                _angle = 0;
            else
                _angle = Mathf.Clamp(angle, 0, maxAngle);
            events.OnMove.Invoke(this, controller);
        }

        /// <summary>
        /// Snap the door to angle
        /// </summary>
        /// <param name="newAngle">the target angle</param>
        private void SetLocalDoorRotation(float newAngle)
        {
            if (hinge == null)
                return;
            _currentAngle = newAngle;
            hinge.localEulerAngles = new Vector3(
                hinge.localEulerAngles.x,
                newAngle * SwingDirection,
                hinge.localEulerAngles.z);
        }




        /////////// Designer Layout
        
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;
            DrawDoorGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
                return;

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
                float z = Mathf.Sin(rads) * radius * SwingDirection;

                points.Add(new Vector3(x, 0f, z));
            }

            points.Add(new Vector3(radius, 0f, 0));

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawLineStrip(points.ToArray(), true);
        }
    }
}
