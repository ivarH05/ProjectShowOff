using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    [RequireComponent(typeof(Rigidbody))]
    public class KillPlayerOnTouch : MonoBehaviour
    {
        [SerializeField] float _fadeInAnimationLength = 1f;
        private void OnCollisionEnter(Collision collision)
        {
            var player = collision?.gameObject?.GetComponent<Player.PlayerController>();
            if (player != null)
                UIManager.SetState<DeathUIState>();
        }
    }
}
