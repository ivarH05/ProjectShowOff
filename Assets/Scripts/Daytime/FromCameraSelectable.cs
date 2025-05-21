using System;
using UnityEngine;

namespace Daytime
{
    /// <summary>
    /// Designates an object as selectable from the daytime camera. Should 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FromCameraSelectable : MonoBehaviour
    {
        public Action OnHoverStart;
        public Action WhileHoverFixedUpdate;
        public Action OnHoverEnd;
        public Action OnClicked;

        private void Awake()
        {
            gameObject.layer = 7;
        }
    }
}
