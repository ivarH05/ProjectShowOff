using Interactables;
using UnityEngine;

namespace UI
{
    public abstract class UIEvent
    {

    }

    public class NoteUIEvent : UIEvent
    {
        public NoteData noteData;
    }
}
