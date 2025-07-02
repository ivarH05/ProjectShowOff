using DialogueSystem;
using Player;
using UnityEngine;

namespace Daytime
{
    [RequireComponent(typeof(FromCameraSelectable))]
    public class SelectableHouse : MonoBehaviour
    {
        [SerializeField] Transform LookAtHouseTransform;
        [SerializeField] DialogueSet DialogueSet;
        [SerializeField] GameObject Renderer;
        [SerializeField] bool showAtNight = false;
        [SerializeField] bool hoverOnly = false;

        private Dialogue currentDialogue;

        FromCameraSelectable _selectable;
        CameraMouseSelector _selector;
        FollowTransform _follow;
        (Vector3, Quaternion) _previousTransform;

        private void OnEnable()
        {
            _selectable = GetComponent<FromCameraSelectable>();
            _selectable.OnHoverStart.AddListener(OnHover);
            _selectable.OnHoverEnd.AddListener(OnHoverEnd);
            if(!hoverOnly)
                _selectable.OnClicked.AddListener(OnClick);
        }

        public void RecalculateDialogue()
        {
            currentDialogue = DialogueSet?.GetDialogue();
            if (currentDialogue == null || (TimeHandler.IsNight() && !showAtNight))
                this.enabled = false;
            else
                this.enabled = true;
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

        public void OnHover()
        {
            if (Renderer == null) return;
            SetTagRecursive(Renderer, 8);
        }

        void OnHoverEnd()
        {
            if (Renderer == null) return;
            SetTagRecursive(Renderer, 1);
        }

        void SetTagRecursive(GameObject go, int mask)
        {
            go.layer = mask;
            foreach(Transform t in go.transform) 
                SetTagRecursive(t.gameObject, mask);
        }

        public void OnClick()
        {
            if (TimeHandler.IsNight())
                return;

            _follow = Camera.main.GetComponent<FollowTransform>();
            _previousTransform = (_follow.ToFollow.position, _follow.ToFollow.rotation);
            _follow.ToFollow.position = LookAtHouseTransform.transform.position;
            _follow.ToFollow.rotation = LookAtHouseTransform.transform.rotation;
            _selector = Camera.main.GetComponent<CameraMouseSelector>();
            _selector.enabled = false;

            DialoguePlayer.StartNewDialogue(currentDialogue, ResetCamera);
        }

        public void SetHoverOnly(bool value) => hoverOnly = value;
    }
}
