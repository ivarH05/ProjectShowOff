using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    public class Door : MonoBehaviour
    {
        [Header("Setup")]
        public Transform hinge;

        [Header("Settings")]
        public float StartAngle;
        public float MaxAngle = 120;
        private void OnDrawGizmos()
        {
            DrawDoorGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (MaxAngle < 0 || MaxAngle > 180)
                Gizmos.color = Color.red;

            DrawDoorGizmos();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //hinge.transform.eulerAngles += new Vector3(0, Input.GetAxis("Mouse Y"), 0);
        }

        private void DrawDoorGizmos()
        {
            const float radius = 0.8f;

            List<Vector3> points = new List<Vector3>() { Vector3.zero };

            for (float a = MaxAngle; a > 0; a -= 10)
            {

                float rads = a * Mathf.Deg2Rad;
                float x = Mathf.Cos(rads) * radius;
                float z = Mathf.Sin(rads) * radius;

                points.Add(new Vector3(x, 0f, z));
            }
            points.Add(new Vector3(radius, 0f, 0));

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawLineStrip(points.ToArray(), true);
        }
    }
}
