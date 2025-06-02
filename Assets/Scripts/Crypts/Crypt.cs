using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Crypts
{
    public class Crypt : MonoBehaviour
    {
        public float height;

        [HideInInspector] public bool isEditing = false;
        [SerializeField] private List<Vector2> EditorTiles;

        private bool _isSelecting = true;
        private const float tileSize = 0.5f;


        private static List<Vector3> _tiles = new List<Vector3>();

        void Start()
        {
            for (int i = 0; i < EditorTiles.Count; i++)
            {
                Vector2 tile = EditorTiles[i];
                _tiles.Add(new Vector3(tile.x, height, tile.y));
            }
        }

        /// <summary>
        /// Get the closest point on the full Crypt Grid
        /// </summary>
        /// <param name="point">world position</param>
        /// <returns>the gridbased worldposition of the closest tile to the worldposition</returns>
        public static Vector3 GetClosestPoint(Vector3 point)
        {
            Vector3 tile = WorldPointToTile(point);

            Vector3 closestPoint = Vector3.zero;
            float minDistance = float.MaxValue;
            for (int i = 0; i < _tiles.Count; i++)
            {
                float distance = Vector3.Distance(tile, _tiles[i]);
                if (distance > minDistance)
                    continue;
                minDistance = distance;
                closestPoint = _tiles[i];
            }

            return TileToWorldPoint(closestPoint);
        }

        public void EditorMouseDown(Vector2 mousePosition)
        {
            Vector2 pos = GetAimingTile(mousePosition);
            _isSelecting = !EditorTiles.Contains(pos);

            SetTile(pos, _isSelecting);
        }
        public void EditorMouse(Vector2 mousePosition) => SetTile(GetAimingTile(mousePosition), _isSelecting);
        public void EditorMouseUp(Vector2 mousePosition) => SetTile(GetAimingTile(mousePosition), _isSelecting);

        private Vector2 GetAimingTile(Vector2 mousePosition)
        {
    #if UNITY_EDITOR
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            Plane XZPlane = new Plane(Vector3.up, new Vector3(0, height, 0));

            if (XZPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                Vector3 tiledPoint = WorldPointToTile(worldPoint);
                return new Vector2(tiledPoint.x + 0.5f, tiledPoint.z + 0.5f);
            }
    #endif
            return Vector2.zero;
        }

        public static Vector3 WorldPointToTile(Vector3 worldPoint)
        {
            Vector3 localPoint = new Vector3(worldPoint.x / tileSize, worldPoint.y, worldPoint.z / tileSize);
            localPoint = new Vector3(Mathf.Floor(localPoint.x), localPoint.y, Mathf.Floor(localPoint.z));

            return localPoint;
        }

        public static Vector3 TileToWorldPoint(Vector3 tile)
        {
            Vector3 result = new Vector3(tile.x * tileSize, tile.y, tile.z * tileSize);

            return result;
        }

        private void SetTile(Vector2 tile, bool value)
        {
            if (value)
            {
                if (EditorTiles.Contains(tile))
                    return;
                EditorTiles.Add(tile);
            }
            else
            {
                if (!EditorTiles.Contains(tile))
                    return;
                EditorTiles.Remove(tile);
            }
    #if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate();
            EditorApplication.RepaintProjectWindow();
    #endif
        }

        private void OnDrawGizmosSelected()
        {
            if (isEditing)
                DrawGrid(new Color(0.3f, 0.3f, 1f), true);
            else
                DrawGrid(new Color(0.55f, 0.55f, 0.8f), true);
        }
        private void OnDrawGizmos()
        {
            DrawGrid(new Color(0.2f, 0.2f, 1f), false);
        }

        void DrawGrid(Color color, bool fill = false)
        {
            if(fill)
                color.a = 0.5f;
            Gizmos.color = color;
            foreach (Vector2 tile in EditorTiles)
            {
                DrawTile(tile, fill);
            }
        }

        void DrawTile(Vector3 tile, bool fill = false)
        {
            Vector3 pos = tile * tileSize;
            Gizmos.DrawWireCube(new Vector3(pos.x, height, pos.y), new Vector3(tileSize, 0.05f, tileSize));
            if(fill)
                Gizmos.DrawCube(new Vector3(pos.x, height, pos.y), new Vector3(tileSize, 0.05f, tileSize));
        }
    }
}
