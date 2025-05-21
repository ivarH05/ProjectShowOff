using Player;
using UnityEngine;

namespace Daytime
{
    [RequireComponent(typeof(FromCameraSelectable))]
    public class SelectableHouse : MonoBehaviour
    {
        [SerializeField] Transform LookAtHouseTransform;
        [SerializeField] DialogueRoot DialogueToShow;

        FromCameraSelectable _selectable;
        CameraMouseSelector _selector;
        FollowTransform _follow;
        (Vector3, Quaternion) _previousTransform;

        private void OnEnable()
        {
            _selectable = GetComponent<FromCameraSelectable>();
            _selectable.OnClicked += OnClick;
        }
        private void OnDisable()
        {
            _selectable.OnClicked -= OnClick;
        }

        public void ResetCamera()
        {
            (_follow.ToFollow.position, _follow.ToFollow.rotation) = _previousTransform;
            _selector.enabled = true;
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
