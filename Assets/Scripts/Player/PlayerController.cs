using GameManagement;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Interactables;
using Player.InventoryManagement;
using UnityEngine.AI;

namespace Player
{
    /// <summary>
    /// Controls the player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(Inventory))]
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public MovementStrategy MoveStrategy { get; private set; }
        [field: SerializeField] public MouseStrategy MouseStrategy { get; private set; }
        [field: SerializeField] public InteractStrategy InteractStrategy { get; private set; }
        [field: SerializeField] public Transform CameraTransform { get; private set; }
        [field: SerializeField] public Transform CrouchTransform { get; private set; }

        [SerializeField] float _coyoteTime = .2f;
        [SerializeField] float _fastCrouchThresholdSeconds = .2f;

        public Inventory Inventory { get; private set; }
        public Rigidbody Body {get; private set;}
        public CapsuleCollider MainCollider { get; private set;}
        public float CharacterHeight { get; private set; }
        /// <summary>
        /// Relative to the top of the MainCollider.
        /// </summary>
        public float EyeOffset { get; private set; }
        public bool IsGrounded => _timeSinceLastFootCollider <= _coyoteTime;
        public bool UncoyotedGrounded => _timeSinceLastFootCollider <= 0;
        public bool SprintHeld { get; private set; }
        public CrouchState Crouch { get; private set; }
        public event Action OnFastCrouch;
        public bool AimHeld { get; private set; }

        [SerializeField] private bool LockCursor;

        Vector2 _currentPlayerDirection;
        Vector2 _previousLookVector;
        bool _attackHeld;
        
        int _collidersInFootTrigger;
        float _timeSinceLastFootCollider = 0;
        double _timeCrouchStartHeld = float.MinValue;

        Vector3 lastPosition;

        public Interactable ActiveInteractable { get { return InteractStrategy?.activeInteractable; } }

        private void Awake() { PlayerManager.RegisterPlayer(this); }
        private void OnDestroy() { PlayerManager.UnregisterPlayer(this); }

        private void Start()
        {
            if (LockCursor)
                Cursor.lockState = CursorLockMode.Locked;
            lastPosition = transform.position;

            Inventory = GetComponent<Inventory>();
            Body = GetComponent<Rigidbody>();
            MainCollider = GetComponent<CapsuleCollider>();
            CharacterHeight = MainCollider.height;
            EyeOffset = CrouchTransform.localPosition.y - MainCollider.height * .5f;

            MoveStrategy ??= GetComponent<Walk>();
            MoveStrategy?.StartStrategy(this);
            MouseStrategy ??= GetComponent<LookAround>();
            MouseStrategy?.StartStrategy(this);
            InteractStrategy ??= GetComponent<StandardInteract>();
            InteractStrategy?.StartStrategy(this);
        }

        private void Update()
        {
            if(_attackHeld)
                InteractStrategy?.OnAttack(this);

            /*if (!NavMesh.SamplePosition(transform.position - new Vector3(0, 0.9f, 0), out NavMeshHit hit, 0.25f, NavMesh.AllAreas))
                Body.position = lastPosition;
            else
                lastPosition = hit.position + new Vector3(0, 0.9f, 0);*/
        }

        /// <summary>
        /// Switches the mouse strategy to another one.
        /// </summary>
        /// <param name="newStrategy">The strategy to use instead of the current one.</param>
        public void SwitchMouseStrategy<T>() where T : MouseStrategy 
        {
            MouseStrategy newStrategy = GetComponent<T>();
            if (newStrategy == null)
            {
                Debug.LogError($"Missing required component of type {typeof(T)} on GameObject. Cannot switch strategy.");
                return;
            }

            MouseStrategy?.StopStrategy(this);
            MouseStrategy = newStrategy;
            newStrategy.StartStrategy(this);
        }

        /// <summary>
        /// Switches the movement strategy to another one.
        /// </summary>
        /// <param name="newStrategy">The strategy to use instead of the current one.</param>
        public void SwitchMovementStrategy<T>() where T : MovementStrategy
        {
            MovementStrategy newStrategy = GetComponent<T>();
            if (newStrategy == null)
            {
                Debug.LogError($"Missing required component of type {typeof(T)} on GameObject. Cannot switch strategy.");
                return;
            }

            MoveStrategy?.StopStrategy(this);
            MoveStrategy = newStrategy;
            newStrategy?.StartStrategy(this);
        }

        public void SwitchInteractStrategy<T>() where T : InteractStrategy
        {
            InteractStrategy newStrategy = GetComponent<T>();
            if (newStrategy == null)
            {
                Debug.LogError($"Missing required component of type {typeof(T)} on GameObject. Cannot switch strategy.");
                return;
            }

            InteractStrategy?.StopStrategy(this);
            InteractStrategy = newStrategy;
            newStrategy?.StartStrategy(this);
        }

        public void ClearInteractStrategy()
        {
            InteractStrategy?.StopStrategy(this);
            InteractStrategy = null;
        }

        public void DisableMovement()
        {
            MoveStrategy?.StopStrategy(this);
            MoveStrategy = null;
        }

        public void MovePosition(Vector3 position)
        {
            Body.isKinematic = true;
            transform.position = position;
            Body.isKinematic = false;
            Body.MovePosition(position);
        }

        public void MoveRotation(Quaternion rot)
        {
            Body.isKinematic = true;
            transform.rotation = rot;
            Body.isKinematic = false;
            Body.MoveRotation(rot);
        }

        public void OnMove(InputAction.CallbackContext context) => _currentPlayerDirection = context.ReadValue<Vector2>();
        public void OnLook(InputAction.CallbackContext context)
        {
            var val = context.ReadValue<Vector2>();
            MouseStrategy?.OnLook(this, val);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (AimHeld)
            {
                Inventory.OnThrow(context);
                return;
            }

            if (context.started)
            {
                _attackHeld = true;
                InteractStrategy?.OnAttackStart(this);
            }
            if (context.canceled)
            {
                _attackHeld = false;
                InteractStrategy?.OnAttackStop(this);
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (_attackHeld)
                return;

            if (context.started)
                AimHeld = true;
            if (context.canceled)
                AimHeld = false;
        }

        public void OnPeekLeft(InputAction.CallbackContext context)
        {
            if (context.started)
                MouseStrategy.OnPeekLeftStart(this);
            if (context.canceled)
                MouseStrategy.OnPeekLeftStop(this);
        }
        public void OnPeekRight(InputAction.CallbackContext context)
        {
            if (context.started)
                MouseStrategy.OnPeekRightStart(this);
            if (context.canceled)
                MouseStrategy.OnPeekRightStop(this);
        }

        public void StopInteraction()
        {
            _attackHeld = false;
            InteractStrategy?.OnAttackStop(this);
        }

        public void OnAttackSecondary(InputAction.CallbackContext context) 
        {
            if(context.started)
                InteractStrategy?.OnAttackSecondary(this);
        }
        public void OnSprint(InputAction.CallbackContext context)
        {
            if(context.started)
                SprintHeld = true;
            if(context.canceled)
                SprintHeld = false;
        }
        public void OnCrouch(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                if (Crouch != CrouchState.CrouchFast)
                {
                    _timeCrouchStartHeld = Time.realtimeSinceStartupAsDouble;
                    Crouch = CrouchState.CrouchSlow;
                }
            }
            if (context.canceled)
            {
                if (_timeCrouchStartHeld > Time.realtimeSinceStartupAsDouble - _fastCrouchThresholdSeconds)
                {
                    Crouch = CrouchState.CrouchFast;
                    OnFastCrouch?.Invoke();
                }
                else Crouch = CrouchState.Standing;
            }
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.started)
                MoveStrategy?.OnJump(this);
        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
                InteractStrategy.OnInteract(this);
        }

        private void FixedUpdate()
        {
            if(_collidersInFootTrigger < 1)
                _timeSinceLastFootCollider += Time.deltaTime;

            Vector3 res = default;
            res += transform.forward * _currentPlayerDirection.y;
            res += transform.right * _currentPlayerDirection.x;
            MoveStrategy?.OnMoveUpdate(this, res, SprintHeld, Crouch);
        }

        private void OnTriggerEnter(Collider other)
        {
            _collidersInFootTrigger++;
            _timeSinceLastFootCollider = 0;
        }
        private void OnTriggerExit(Collider other) => _collidersInFootTrigger--;
    }
}
