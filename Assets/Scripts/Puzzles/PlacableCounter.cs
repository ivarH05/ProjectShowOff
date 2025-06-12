using Interactables;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Puzzles
{
    public class PlacableCounter : MonoBehaviour
    {
        public Placable[] placables;
        public int placedCounter = 0;

        public UnityEvent<PlacableCounter> OnComplete = new UnityEvent<PlacableCounter>();

        void Start()
        {
            for (int i = 0; i < placables.Length; i++)
            {
                placables[i].events.OnPlace.AddListener(RegisterSlot);
                placables[i].events.OnRemove.AddListener(UnregisterSlot);
            }
        }

        void RegisterSlot(Placable placable, PlayerController player)
        {
            placedCounter++;
            if (placedCounter == placables.Length)
                OnComplete.Invoke(this);
        }

        void UnregisterSlot(Placable placable, PlayerController player)
        {
            placedCounter--;
        }
    }
}
