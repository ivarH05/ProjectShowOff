using UI;
using UnityEngine;

namespace Crypts
{
    public class CryptExit : MonoBehaviour
    {
        private void Awake()
        {
            Collider c = GetComponent<Collider>();
            if (c == null || !c.isTrigger)
            {
                Debug.LogError("CryptExit needs a trigger collider to function.");
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            UIManager.SetState<TryExitCryptUIState>();
        }
    }
}
