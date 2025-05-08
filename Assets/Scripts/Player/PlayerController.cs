using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Controls the player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public MovementStrategy MoveStrategy { get; private set; }
        [field: SerializeField] public MouseStrategy MouseStrategy { get; private set; }
        [field: SerializeField] public Transform CameraTransform { get; private set; }
        [SerializeField] float _coyoteTime = .2f;

        public Rigidbody Body {get; private set;}
        public CapsuleCollider MainCollider { get; private set;}
        public bool IsGrounded => _timeSinceLastFootCollider <= _coyoteTime;
        public bool UncoyotedGrounded => _timeSinceLastFootCollider <= 0;

        PlayerInput _input;
        Vector2 _currentPlayerDirection;
        bool _sprintHeld;
        bool _crouchHeld;
        int _collidersInFootTrigger;
        float _timeSinceLastFootCollider = 0;

        private void Start()
        {
            Body = GetComponent<Rigidbody>();
            MainCollider = GetComponent<CapsuleCollider>();
            _input = GetComponent<PlayerInput>();

            MoveStrategy ??= GetComponent<MovementStrategy>();
            MoveStrategy?.StartStrategy(this);
            MouseStrategy ??= GetComponent<MouseStrategy>();
            MouseStrategy?.StartStrategy(this);
        }

        /// <summary>
        /// Switches the movement strategy to another one.
        /// </summary>
        /// <param name="newStrategy">The strategy to use instead of the current one.</param>
        public void SwitchStrategy(MovementStrategy newStrategy)
        {
            MoveStrategy?.StopStrategy(this);
            MoveStrategy = newStrategy;
            newStrategy.StartStrategy(this);
        }
        /// <summary>
        /// Switches the movement strategy to another one.
        /// </summary>
        /// <param name="newStrategy">The strategy to use instead of the current one.</param>
        public void SwitchStrategy(MouseStrategy newStrategy)
        {
            MouseStrategy?.StopStrategy(this);
            MouseStrategy = newStrategy;
            newStrategy.StartStrategy(this);
        }

        public void OnMove(InputAction.CallbackContext context) => _currentPlayerDirection = context.ReadValue<Vector2>();
        public void OnLook(InputAction.CallbackContext context) => MouseStrategy?.OnLook(this, context.ReadValue<Vector2>());
        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.started)
                MouseStrategy?.OnAttack(this);
        }
        public void OnAttackSecondary(InputAction.CallbackContext context) 
        {
            if(context.started)
                MouseStrategy?.OnAttackSecondary(this);
        }
        public void OnSprint(InputAction.CallbackContext context)
        {
            if(context.started)
                _sprintHeld = true;
            if(context.canceled)
                _sprintHeld = false;
        }
        public void OnCrouch(InputAction.CallbackContext context)
        {
            if(context.started)
                _crouchHeld = true;
            if(context.canceled)
                _crouchHeld = false;
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.started)
                MoveStrategy?.OnJump(this);
        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            if(context.started)
                MouseStrategy.OnInteract(this);
        }

        private void FixedUpdate()
        {
            if(_collidersInFootTrigger < 1)
                _timeSinceLastFootCollider += Time.deltaTime;

            Vector3 res = default;
            res += transform.forward * _currentPlayerDirection.y;
            res += transform.right * _currentPlayerDirection.x;
            MoveStrategy?.OnMoveUpdate(this, res, _sprintHeld, _crouchHeld);
        }

        private void OnTriggerEnter(Collider other)
        {
            _collidersInFootTrigger++;
            _timeSinceLastFootCollider = 0;
        }
        private void OnTriggerExit(Collider other) => _collidersInFootTrigger--;
    }
}
