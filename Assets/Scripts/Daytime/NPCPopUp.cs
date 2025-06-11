using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Daytime
{
    public enum PopUpState { Closed, Small, Big }

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class NPCPopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Character character;
        [SerializeField] private GameObject SmallState;
        [SerializeField] private GameObject BigState;
        [Space]
        [SerializeField] private Image CharacterIconSmall;
        [SerializeField] private Image CharacterIconBig;
        [SerializeField] private TMP_Text CharacterName;
        private bool _mouseOver = false;

        private Image image;

        public PopUpState State { get => _state; set => SetState(value); }
        [SerializeField] private PopUpState _state;

        private new RectTransform transform;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            transform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            SetCharacter(character);
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 targetSize = Vector2.zero;
            float PixelRadius = 3;
            switch(_state)
            {
                case PopUpState.Closed:
                    SmallState.SetActive(false);
                    BigState.SetActive(false);
                    break;
                case PopUpState.Small:
                    SmallState.SetActive(true);
                    BigState.SetActive(false);
                    targetSize = new Vector2(100, 100); 
                    PixelRadius = 6.5f;  
                    break;
                case PopUpState.Big:
                    SmallState.SetActive(false);
                    BigState.SetActive(true);
                    targetSize = new Vector2(300, 200); 
                    PixelRadius = 5; 
                    break;
            }

            transform.sizeDelta = Vector2.Lerp(transform.sizeDelta, targetSize, Time.deltaTime * 15);
            image.pixelsPerUnitMultiplier = Mathf.Lerp(image.pixelsPerUnitMultiplier, PixelRadius, Time.deltaTime * 15);


            if (State == PopUpState.Closed)
                return;

            if (_mouseOver)
                State = PopUpState.Big;
            else
                State = PopUpState.Small;
        }

        public void SetCharacter(Character character)
        {
            CharacterIconSmall.sprite = character.sprite;
            CharacterIconBig.sprite = character.sprite;
            CharacterName.text = character.name;
        }

        public void SetState(PopUpState state)
        {
            _state = state;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOver = true;
            Debug.Log("Mouse enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOver = false;
            Debug.Log("Mouse exit");
        }
    }
}

