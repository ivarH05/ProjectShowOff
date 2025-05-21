using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    [RequireComponent(typeof(Rigidbody))]
    public class KillPlayerOnTouch : MonoBehaviour
    {
        [SerializeField] CanvasGroup _deathScreen;
        [SerializeField] float _fadeInAnimationLength = 1f;
        private void OnCollisionEnter(Collision collision)
        {
            var player = collision?.gameObject?.GetComponent<Player.PlayerController>();
            if (player != null)
            {
                player.enabled = false;
                _deathScreen.blocksRaycasts = true;
                _deathScreen.interactable = true;
                StartCoroutine(FadeInDeathscreen());
                var playerInput = collision.gameObject.GetComponent<PlayerInput>();
                if(playerInput != null) playerInput.enabled = false;
            }

            IEnumerator FadeInDeathscreen()
            {
                var sc = _deathScreen;
                float deltaAlpha = 1 / _fadeInAnimationLength;
                sc.alpha += deltaAlpha * Time.deltaTime;
                while(sc.alpha < 1)
                {
                    sc.alpha += deltaAlpha * Time.deltaTime;
                    yield return null;
                }
            }
        }
    }
}
