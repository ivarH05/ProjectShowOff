using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NoteUIState : UIState
    {
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private Image background;
        [SerializeField] private Sprite[] noteSprites;
        public override void OnEvent<T>(T context)
        {
            if (context is not NoteUIEvent e)
                return;
            background.sprite = noteSprites[Random.Range(0, noteSprites.Length)];
            contentText.text = e.noteData.content;
        }
    }
}