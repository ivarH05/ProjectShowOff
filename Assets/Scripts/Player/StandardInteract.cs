using Player;
using UnityEngine;

namespace Player
{
    public class StandardInteract : InteractStrategy
    {
        [SerializeField] float _interactDistance = 1.5f;

        public override void StartStrategy(PlayerController controller) { }

        public override void OnAttackSecondary(PlayerController controller) { }

        public override void OnAttackStart(PlayerController controller)
        {
            if (activeInteractable != null)
                return;

            activeInteractable = GetAimingInteractable(controller.CameraTransform);
            activeInteractable?.OnUseStart(controller);
        }
        public override void OnAttack(PlayerController controller) { activeInteractable?.OnUse(controller); }
        public override void OnAttackStop(PlayerController controller)
        {
            activeInteractable?.OnUseStop(controller);
            activeInteractable = null;
        }

        public override void OnInteract(PlayerController controller)
        {
            if (activeInteractable != null)
                return;

            GetAimingInteractable(controller.CameraTransform)?.OnInteract(controller);
        }

        public override void StopStrategy(PlayerController controller) { }

        public Interactable GetAimingInteractable(Transform cameraTransform)
        {
            RaycastHit hit;
            if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, _interactDistance, (1 << 6)))
                return null;

            Interactable result = hit.transform.GetComponent<Interactable>();
            if (result != null)
                return result;

            return hit.transform.GetComponentInParent<Interactable>();
        }
    }
}
