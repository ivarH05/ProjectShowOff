using System;
using UnityEngine;

namespace CryptBuilder
{
    [SelectionBase, ExecuteInEditMode]
    public class CryptRoom : MonoBehaviour
    {
        public CryptRoomStyle Style;
        [NonSerialized] public LOD CurrentLOD;

        private void Awake()
        {
            GameObject gen = DisabledAtDistanceChildren;
        }
        public GameObject DisabledAtDistanceChildren
        {
            get
            {
                if(_disabledAtDistance == null)
                {
                    _disabledAtDistance = new("Disabled at distance");
                    _disabledAtDistance.transform.SetParent(transform, false);
                }
                return _disabledAtDistance;
            }
        }
        [SerializeField, HideInInspector] GameObject _disabledAtDistance;

        public GameObject GeneratedChildren
        {
            get
            {
                if(_generatedChildren == null)
                {
                    _generatedChildren = new GameObject("Generated children");
                    _generatedChildren.hideFlags = HideFlags.HideAndDontSave;
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

        private void OnDestroy()
        {
            if (_generatedChildren != null)
                DestroyImmediate(_generatedChildren);
        }

        public enum LOD
        {
            None, LowDetail, HighDetail
        }
    }
}
