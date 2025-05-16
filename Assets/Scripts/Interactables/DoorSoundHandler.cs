using AdvancedSound;
using Player;
using UnityEngine;

namespace Interactables
{
    public class DoorSoundHandler : MonoBehaviour
    {
        [SerializeField] SoundPlayer Open;
        [SerializeField] SoundPlayer Creak;
        [SerializeField] SoundPlayer Close;
        [SerializeField] float CreakFrequencyMultiplier;
        [SerializeField, Range(0f, .9f)] float SpeedPitchMultiplier = 1;

        Door _door;
        float _previousDoorAngle;
        float _creakCounter;

        private void OnEnable()
        {
            _door = transform.parent.GetComponent<Door>();
            _door.events.OnOpen?.AddListener(DoorOpen);
            _door.events.OnClose?.AddListener(DoorClose);
            _previousDoorAngle = _door.CurrentAngle;
        }
        private void OnDisable()
        {
            _door.events.OnOpen?.RemoveListener(DoorOpen);
            _door.events.OnClose?.RemoveListener(DoorClose);
        }

        private void FixedUpdate()
        {
            float angleDelta = _door.CurrentAngle - _previousDoorAngle;
            HandleCreak(Mathf.Abs(angleDelta));
            _previousDoorAngle = _door.CurrentAngle;
        }

        // 𝒞𝑅𝐸𝒜𝒦 level: ∞
        void HandleCreak(float delta)
        {
            _creakCounter += delta * CreakFrequencyMultiplier * .01f; // magic constant to get reasonable values
            if(_creakCounter > 1)
            {
                _creakCounter = 0;
                var logvol = Mathf.Log(delta) + 2;
                logvol = Mathf.Clamp01(logvol * .3f);
                // logarithmic scale because the speed can vary quite a lot
                Creak?.Play(logvol, 1 + (logvol - .5f) * SpeedPitchMultiplier);
            }
        }

        void DoorOpen(Door d, PlayerController p) => Open?.Play();
        void DoorClose(Door d, PlayerController p) => Close?.Play();
    }
}
