using System;
using UnityEngine;
using UnityEngine.Events;

namespace Daytime
{
    /// <summary>
    /// Designates an object as selectable from the daytime camera. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FromCameraSelectable : MonoBehaviour
    {
        public UnityEvent OnHoverStart;
        public UnityEvent WhileHoverFixedUpdate;
        public UnityEvent OnHoverEnd;
        public UnityEvent OnClicked;

        private void Awake()
        {
            gameObject.layer = 7;
        }
    }
}
