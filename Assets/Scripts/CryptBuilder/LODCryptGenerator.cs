using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryptBuilder
{
    [RequireComponent(typeof(Builder))]
    public class LODCryptGenerator : MonoBehaviour
    {
        [SerializeField] float _lowDetailRange = 30;
        [SerializeField] float _highDetailRange = 10;

        private void Awake()
        {
            
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(LODCryptGenerator))]
        class LODGenEditor : Editor
        {

            void OnSceneGUI()
            {
                var b = (LODCryptGenerator)target;
                var build = b.GetComponent<Builder>();
                Handles.matrix = Matrix4x4.TRS(build.transform.position, Quaternion.identity, Vector3.one);

                if (TryTracePlaneFromMouse(build, out Vector2 mousePos))
                {
                    var off = new Vector2(b._lowDetailRange, b._lowDetailRange);
                    var mouseBB = new BoundingBox(mousePos - off, mousePos + off);
                    Handles.color = new(.4f, .7f, 0, .8f);
                    CryptHandles.DrawBoundingBox(mouseBB);
                    
                    var rects = build.RectangleTree.GetRectanglesIntersectingBox(mouseBB);
                    foreach ((int nodeI, int rectI) in rects)
                    {
                        CryptHandles.DrawBoundingBox(build.RectangleTree.Nodes[nodeI].Rectangles[rectI].GetBounds(), 1);
                    }

                    off = new Vector2(b._highDetailRange, b._highDetailRange);
                    mouseBB = new BoundingBox(mousePos - off, mousePos + off);
                    Handles.color = new(.1f, .6f, 0.8f, .8f);
                    CryptHandles.DrawBoundingBox(mouseBB);

                    rects = build.RectangleTree.GetRectanglesIntersectingBox(mouseBB);
                    foreach ((int nodeI, int rectI) in rects)
                    {
                        CryptHandles.DrawBoundingBox(build.RectangleTree.Nodes[nodeI].Rectangles[rectI].GetBounds(), 2);
                    }
                }
            }
            static bool TryTracePlaneFromMouse(Builder b, out Vector2 position)
            {
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                position = default;
                ray.origin -= b.transform.position;
                float t = ray.origin.y / -ray.direction.y;
                if (t < 0 || float.IsNaN(t))
                    return false;

                position = (ray.origin + ray.direction * t).To2D();
                return true;
            }
        }
#endif
    }
}
