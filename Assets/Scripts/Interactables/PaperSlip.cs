using Player;
using Player.InventoryManagement;
using UI;
using UnityEngine;

namespace Interactables
{
    public class Note : Interactable
    {
        public NoteData note;

        private void Awake()
        {
        }

        public override void OnInteract(PlayerController controller)
        {
            UIManager.SetState<NoteUIState>();
            UIManager.CallUIEvent(new NoteUIEvent() { noteData = note } );
            Destroy(gameObject);
        }

        public override void OnUseStart(PlayerController controller) { }
        public override void OnUse(PlayerController controller) { }
        public override void OnUseStop(PlayerController controller) { }
    }
}
