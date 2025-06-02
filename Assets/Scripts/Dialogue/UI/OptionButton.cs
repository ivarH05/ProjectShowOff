using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
    public class OptionButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text text;

        public void Setup(OptionData optionData, Action<int> OnClick)
        {
            button.onClick.AddListener(() => { OnClick(optionData.index); } );
            text.text = optionData.ResponseText;
        }
    }
}
