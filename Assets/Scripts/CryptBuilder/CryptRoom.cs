using UnityEngine;

namespace CryptBuilder
{
    public class CryptRoom : MonoBehaviour
    {
        public CryptRoomStyle Style;
        public GameObject GeneratedChildren
        {
            get
            {
                if(_generatedChildren == null)
                {
                    _generatedChildren = new GameObject("Generated children (may be deleted)");
                    _generatedChildren.transform.SetParent(transform, false);
                }
                return _generatedChildren;
            }
        }
        GameObject _generatedChildren;
    }
}
