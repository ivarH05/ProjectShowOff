using UnityEngine;

namespace CryptBuilder
{
    public class CryptRoom : MonoBehaviour
    {
        public CryptRoomStyle Style;
        public LOD CurrentLOD;

        public GameObject GeneratedChildren
        {
            get
            {
                if(_generatedChildren == null)
                {
                    _generatedChildren = new GameObject("Generated children");
                    _generatedChildren.transform.SetParent(transform, false);
                }
                return _generatedChildren;
            }
        }
        [SerializeField, HideInInspector] GameObject _generatedChildren;

        public GameObject Colliders
        {
            get
            {
                if(_colliders == null)
                {
                    _colliders = new GameObject("Generated colliders");
                    _colliders.transform.SetParent(transform, false);
                }
                return _colliders;
            }
        }
        [SerializeField, HideInInspector] GameObject _colliders;

        public enum LOD
        {
            None, LowDetail, HighDetail
        }
    }
}
