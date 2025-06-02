using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.UI
{
    public class OptionButtonManager : MonoBehaviour
    {
        [SerializeField] private GameObject OptionButtonPrefab;
        private List<OptionButton> buttons = new List<OptionButton>();

        public void Setup(List<OptionData> data, Action<int> onClick)
        {
            gameObject.SetActive(true);
            Clear();

            foreach (OptionData option in data)
            {
                GameObject obj = Instantiate(OptionButtonPrefab, transform);
                OptionButton button = obj.GetComponent<OptionButton>();
                button.Setup(option, onClick);
                buttons.Add(button);
            }
        }

        private void Clear()
        {
            for (int i = 0; i < buttons.Count; i++)
                Destroy(buttons[i].gameObject);
            buttons.Clear();
        }
    }
}
