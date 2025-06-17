using TMPro;
using UnityEngine;

namespace UI
{
    public class NoteUIState : UIState
    {
        [SerializeField] private TMP_Text contentText;
        public override void OnEvent<T>(T context)
        {
            if (context is not NoteUIEvent e)
                return;
            contentText.text = e.noteData.content;
        }
    }
}