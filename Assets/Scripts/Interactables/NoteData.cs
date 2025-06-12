using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    [CreateAssetMenu(fileName = "NoteData", menuName = "Interactables/NoteData")]
    public class NoteData : ScriptableObject
    {
        [TextArea(15, 20)]
        public string content;

        public UnityEvent OnGrab = new UnityEvent();
    }
}