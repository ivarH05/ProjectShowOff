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

        public Rigidbody Body {get; private set;}

        PlayerInput _input;
        Vector2 _currentPlayerDirection;

        private void Start()
        {
            Body = GetComponent<Rigidbody>();
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

        private void FixedUpdate()
        {
            Vector3 res = default;
            res += transform.forward * _currentPlayerDirection.y;
            res += transform.right * _currentPlayerDirection.x;
            MoveStrategy?.OnMove(this, res);
        }
    }
}
