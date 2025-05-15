using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class Crypt : MonoBehaviour
{
    [HideInInspector] public bool isEditing = false;
    private bool _isSelecting = true;

    [SerializeField]private List<Vector2> EditorTiles;

    private HashSet<Vector2> _tiles = new HashSet<Vector2>();
    private const float tileSize = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tiles = EditorTiles.ToHashSet();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EditorMouseDown(Vector2 mousePosition)
    {
        Vector2 pos = GetAimingTile(mousePosition);
        _isSelecting = !EditorTiles.Contains(pos);

        SetTile(pos, _isSelecting);
    }
    public void EditorMouse(Vector2 mousePosition)
    {
        SetTile(GetAimingTile(mousePosition), _isSelecting);
    }
    public void EditorMouseUp(Vector2 mousePosition)
    {
        SetTile(GetAimingTile(mousePosition), _isSelecting);
    }

    Vector2 GetAimingTile(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        Plane localXZPlane = new Plane(transform.up, transform.position);

        if (localXZPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
            localPoint /= tileSize;
            localPoint = Vector3Int.FloorToInt(localPoint);

            return new Vector2(localPoint.x, localPoint.z);
        }
        return Vector2.zero;
    }

    void SetTile(Vector2 tile, bool value)
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
        EditorApplication.QueuePlayerLoopUpdate();
        EditorApplication.RepaintProjectWindow();
    }

    private void OnDrawGizmosSelected()
    {
        if (isEditing)
            DrawEditorGizmos();
        else
            DrawNormalGizmos();
    }

    void DrawNormalGizmos()
    {
        DrawGrid(new Color(0.5f, 0.5f, 1f));
    }

    void DrawEditorGizmos()
    {
        DrawGrid(new Color(0.3f, 0.3f, 1f), true);
    }

    void DrawGrid(Color color, bool wire = false)
    {
        color.a = 0.5f;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = color;
        foreach (Vector2 tile in EditorTiles)
        {
            DrawTile(tile, wire);
        }
    }

    void DrawTile(Vector2 tile, bool wire = false)
    {
        Vector2 pos = tile * tileSize;
        Gizmos.DrawCube(new Vector3(pos.x + 0.5f * tileSize, 0, pos.y + 0.5f * tileSize), new Vector3(tileSize, 0.05f, tileSize));
        if(wire)
            Gizmos.DrawWireCube(new Vector3(pos.x + 0.5f * tileSize, 0, pos.y + 0.5f * tileSize), new Vector3(tileSize, 0.05f, tileSize));
    }
}
