using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerGridMaker))]
public class TowerGridMakerEditor : Editor
{
    private TowerGridMaker gridMaker;
    private const string DEfAULT_SAVE_PATH = "Assets/ScriptableObjects/Grids";

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (gridMaker == null) return;

        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Plane plane = new Plane(Vector3.forward, gridMaker.transform.position);
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (plane.Raycast(ray, out float enter))
            {
                Undo.RecordObject(gridMaker, "Toggle Grid Cell");

                Vector3 worldPos = ray.GetPoint(enter);
                Vector2Int cell = gridMaker.WorldToCell(worldPos);
                gridMaker.ToggleCell(cell);

                EditorUtility.SetDirty(gridMaker);
                e.Use();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        gridMaker = (TowerGridMaker)target;

        EditorGUILayout.HelpBox(
            "In Scene View:\n" +
            "- Left click to toggle cells\n" +
            "- Choose in the 'Editor Brush' below whether to paint Road or Tree\n" +
            "- Undo is available\n" +
            "- Save makes sure to save changes, not necessary if CurrentData isn't null" +
            ", else you can save the new data it in a folder of choice",
            MessageType.Info);

        if (GUILayout.Button("Save"))
        {
            if (gridMaker.CurrentData != null)
            {
                EditorUtility.SetDirty(gridMaker.CurrentData);
                AssetDatabase.SaveAssets();
            }
            else
            {
                CreateAndSaveGridData();
            }
        }

        DrawDefaultInspector();

    }

    private void CreateAndSaveGridData()
    {
        string path = EditorUtility.OpenFolderPanel(
                            title: "Save Grid Data",
                            folder: (string.IsNullOrEmpty(TowerGridMaker.PreviousSaveFolder)) ? Application.dataPath : TowerGridMaker.PreviousSaveFolder,
                            defaultName: ""
                        );

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("No folder selected. Operation cancelled.");
            return;
        }

        if (!path.StartsWith(Application.dataPath))
        {
            Debug.LogError("Please select a folder inside the Assets directory.");
            return;
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        TowerGridMaker.PreviousSaveFolder = path;

        // cut everything except the relative folder path => .../Folder/folder
        // and add "Assets" to the beginning to get the relative path
        string relativeFolderPath = "Assets" + path[Application.dataPath.Length..];

        string relativePath = Path.Combine(relativeFolderPath, TowerGridMaker.DefaultGridDataName + ".asset");

        GridData gridData = CreateInstance<GridData>();
        gridData.Initialize(
            gridMaker.width,
            gridMaker.height,
            gridMaker.cellSize,
            new List<Vector2Int>(gridMaker.roadCells),
            new List<Vector2Int>(gridMaker.treeCells)
        );

        AssetDatabase.CreateAsset(gridData, AssetDatabase.GenerateUniqueAssetPath(relativePath));
        AssetDatabase.SaveAssets();

        // Focus on the created asset in the Project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = gridData;

        // Assign the created GridData to the TowerGridMaker
        gridMaker.CurrentData = gridData;
    }
}
