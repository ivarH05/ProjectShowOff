using UnityEngine;

namespace AdvancedSound
{
    [RequireComponent(typeof(Rigidbody))]
    public class SoundOnCollision : MonoBehaviour
    {
        [SerializeField] SoundPlayer _sound;
        [SerializeField] float _velocityAtMaxLoudness = 1;
        Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        private void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++) 
            {
                var vel = _rb.GetPointVelocity(collision.contacts[i].point);
                if(collision.rigidbody != null)
                    vel -= collision.rigidbody.GetPointVelocity(collision.contacts[i].point);

                float loudness = vel.sqrMagnitude / (_velocityAtMaxLoudness * _velocityAtMaxLoudness);
                _sound.Play(Mathf.Clamp01(loudness));
            }
        }
    }
}
