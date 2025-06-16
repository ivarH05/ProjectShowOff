using Player;
using UnityEngine;

namespace Daytime
{
    [RequireComponent(typeof(FromCameraSelectable))]
    public class SelectableHouse : MonoBehaviour
    {
        [SerializeField] Transform LookAtHouseTransform;
        [SerializeField] DialogueRoot DialogueToShow;
        [SerializeField] GameObject Renderer;

        FromCameraSelectable _selectable;
        CameraMouseSelector _selector;
        FollowTransform _follow;
        (Vector3, Quaternion) _previousTransform;

        private void OnEnable()
        {
            _selectable = GetComponent<FromCameraSelectable>();
            _selectable.OnHoverStart.AddListener(OnHover);
            _selectable.OnHoverEnd.AddListener(OnHoverEnd);
            _selectable.OnClicked.AddListener(OnClick);
        }
        private void OnDisable()
        {
            _selectable.OnClicked.RemoveListener(OnClick);
            _selectable.OnHoverStart.RemoveListener(OnHover);
            _selectable.OnHoverEnd.RemoveListener(OnHoverEnd);
        }

        public void ResetCamera()
        {
            (_follow.ToFollow.position, _follow.ToFollow.rotation) = _previousTransform;
            _selector.enabled = true;
        }

        void OnHover()
        {
            if (Renderer == null) return;
            Renderer.layer = 1 << 8;
        }

        void OnHoverEnd()
        {
            if (Renderer == null) return;
            Renderer.layer = 1;
        }

        void OnClick()
        {
            if(DialogueToShow == null)
            {
                Debug.LogError("The DialogueRoot was missing, and thus dialogue could not be started.", this);
                return;
            }

            _follow = Camera.main.GetComponent<FollowTransform>();
            _previousTransform = (_follow.ToFollow.position, _follow.ToFollow.rotation);
            _follow.ToFollow.position = LookAtHouseTransform.transform.position;
            _follow.ToFollow.rotation = LookAtHouseTransform.transform.rotation;
            _selector = Camera.main.GetComponent<CameraMouseSelector>();
            _selector.enabled = false;

            var dialogueRoot = DialogueToShow;
            dialogueRoot.Enable();
            dialogueRoot.OnDialogueDisable += DisabledDialogue;

            void DisabledDialogue()
            {
                ResetCamera();
                dialogueRoot.OnDialogueDisable -= DisabledDialogue;
            }
        }
    }
}
