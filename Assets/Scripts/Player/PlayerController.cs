using GameManagement;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Interactables;
using Player.InventoryManagement;

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
        
        [SerializeField] float _coyoteTime = .2f;

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
        public bool CrouchHeld { get; private set; }

        Vector2 _currentPlayerDirection;
        Vector2 _previousLookVector;
        bool _attackHeld;
        
        int _collidersInFootTrigger;
        float _timeSinceLastFootCollider = 0;

        public Interactable ActiveInteractable { get { return InteractStrategy.activeInteractable; } }

        private void Awake() { PlayerManager.RegisterPlayer(this); }
        private void OnDestroy() { PlayerManager.UnregisterPlayer(this); }

        private void Start()
        {
            Inventory = GetComponent<Inventory>();
            Body = GetComponent<Rigidbody>();
            MainCollider = GetComponent<CapsuleCollider>();
            CharacterHeight = MainCollider.height;
            EyeOffset = CameraTransform.localPosition.y - MainCollider.height * .5f;

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

        public void DisableMovement()
        {
            MoveStrategy?.StopStrategy(this);
            MoveStrategy = null; 
        }

        public void MovePosition(Vector3 position)
        {
            transform.position = position;
            Body.MovePosition(position);
        }

        public void OnMove(InputAction.CallbackContext context) => _currentPlayerDirection = context.ReadValue<Vector2>();
        public void OnLook(InputAction.CallbackContext context)
        {
            var val = context.ReadValue<Vector2>();
            MouseStrategy?.OnLook(this, val);
        }
        public void OnAttack(InputAction.CallbackContext context)
        {
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
                CrouchHeld = true;
            if(context.canceled)
                CrouchHeld = false;
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
            MoveStrategy?.OnMoveUpdate(this, res, SprintHeld, CrouchHeld);
        }

        private void OnTriggerEnter(Collider other)
        {
            _collidersInFootTrigger++;
            _timeSinceLastFootCollider = 0;
        }
        private void OnTriggerExit(Collider other) => _collidersInFootTrigger--;
    }
}
