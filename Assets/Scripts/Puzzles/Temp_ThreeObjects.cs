using Interactables;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Puzzles
{
    public class Temp_ThreeObjects : MonoBehaviour
    {
        public Placable[] placables;
        public int placedCounter = 0;


        void Start()
        {
            for (int i = 0; i < placables.Length; i++)
                placables[i].events.OnPlace.AddListener(RegisterSlot);
        }


        void Update()
        {
            if (placedCounter == placables.Length)
                SceneManager.LoadScene(0);
        }

        void RegisterSlot(Placable placable, PlayerController player)
        {
            placedCounter++;
        }
    }
}
