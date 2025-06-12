using UnityEngine;

namespace Tools
{
    /// <summary>
    /// its simply that easy
    /// </summary>
    public class DeleteGameObjectInPlay : MonoBehaviour
    {
        void Start()
        {
            Destroy(gameObject);
        }
    }
}
