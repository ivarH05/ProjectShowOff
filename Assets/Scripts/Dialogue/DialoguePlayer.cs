using DialogueSystem.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogueSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class DialoguePlayer : MonoBehaviour
    {
        private static DialoguePlayer _singleton;

        [SerializeField] private OptionButtonManager optionButtonManager;
        [SerializeField] private GameObject nextButton;
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private TMP_Text mainText;
        [SerializeField] private Image characterSprite;
        private AudioSource _audioSource;
        private Dialogue _activeDialogue;
        private NodeData activeNode;

        public Events events = new Events();


        public void Awake()
        {
            gameObject.SetActive(false);
            _singleton = this;
            _audioSource = GetComponent<AudioSource>();
        }

        public static void StartNewDialogue(Dialogue dialogue, Action OnEnd)
        {
            _singleton.events.OnDialogueEnd.AddListener(OnDialogueEnd);
            _singleton.SetDialogue(dialogue);
            _singleton.StartDialogue();

            void OnDialogueEnd(Dialogue dialogue)
            {
                _singleton.events.OnDialogueEnd.RemoveListener(OnDialogueEnd);
                OnEnd(); 
            }
        }

        public void SetSprite(Sprite sprite)
        {
            characterSprite.sprite = sprite;
            characterSprite.transform.localScale = new Vector3(
                (float)sprite.texture.width / sprite.texture.height,
                characterSprite.transform.localScale.y,
                characterSprite.transform.localScale.z);
        }

        public void SetDialogue(Dialogue dialogue)
        {
            _activeDialogue = dialogue;
        }

        public void PlayOneShot(AudioClip clip, float volume, float pitch)
        {
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(clip);
        }

        public void StartDialogue()
        {
            if (_activeDialogue == null)
                return;

            gameObject.SetActive(true);
            activeNode = _activeDialogue.GetStartNode();
            events.OnDialogueStart.Invoke(_activeDialogue);
            Next(0);
        }

        public void EndDialogue()
        {
            if (_activeDialogue == null)
                return;

            events.OnDialogueEnd.Invoke(_activeDialogue);
            //_activeDialogue = null;
            activeNode = null;
            gameObject.SetActive(false);
        }

        public void Next(int index)
        {
            do
            {
                NextNode(index);
                index = 0;
                if (activeNode == null || activeNode.type == NodeType.End)
                {
                    EndDialogue();
                    return;
                }
            }
            while (!(activeNode is TextNodeData));
        }

        private void NextNode(int index)
        {
            if (_activeDialogue == null)
                return;
            if (activeNode == null || activeNode.type == NodeType.End)
                return;

            activeNode = _activeDialogue.ContinueNode(activeNode, index);
            events.OnDialogueNext.Invoke(activeNode);
            if (activeNode != null)
                HandleNode();
        }

        void HandleNode()
        {
            switch (activeNode.type)
            {
                case NodeType.Text:
                    HandleTextNode();
                    break;
                case NodeType.Branch:
                    HandleBranchNode();
                    break;
                case NodeType.Event:
                    HandleEventNode();
                    break;

                default:
                    break;
            }
        }

        public void HandleTextNode()
        {
            if (!(activeNode is TextNodeData data))
                return;
            if(data.Options.Count > 1)
            {
                optionButtonManager.Setup(data.Options, Next);
                nextButton.SetActive(false);
            }
            else
            {
                optionButtonManager.gameObject.SetActive(false);
                nextButton.SetActive(true);
            }
            mainText.text = data.dialogueText;
            characterNameText.text = data.speaker.name;

            SetSprite(data.speaker.GetSprite(data.expression));
        }

        void HandleBranchNode()
        {
            if (!(activeNode is BranchNodeData data))
                return;

            if (data.flag.Get())
                NextNode(0); //warning: due to the order of the node outputs, 0 is true
            else
                NextNode(1); //warning: due to the order of the node outputs, 1 is false
        }

        void HandleEventNode()
        {
            if (!(activeNode is EventNodeData data))
                return;
            data.onRun.Invoke(this);
        }

        [System.Serializable]
        public class Events
        {
            public UnityEvent<Dialogue> OnDialogueStart = new UnityEvent<Dialogue>();
            public UnityEvent<NodeData> OnDialogueNext = new UnityEvent<NodeData>();
            public UnityEvent<Dialogue> OnDialogueEnd = new UnityEvent<Dialogue>();
        }
    }
}
