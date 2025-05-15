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

        Door _door;
        float _previousDoorAngle;

        private void OnEnable()
        {
            _door = transform.parent.GetComponent<Door>();
            _door.events.OnMove?.AddListener(DoorMove);
            _previousDoorAngle = _door.CurrentAngle;
        }
        private void OnDisable()
        {
            _door.events.OnMove?.RemoveListener(DoorMove);
        }

        void DoorMove(Door d, PlayerController p)
        {
            float angleDelta = d.CurrentAngle - _previousDoorAngle;

            _previousDoorAngle = d.CurrentAngle;
        }
    }
}
