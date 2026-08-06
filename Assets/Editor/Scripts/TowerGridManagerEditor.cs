using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerGridManager))]
public class TowerGridManagerEditor : Editor
{
    private TowerGridManager gridManager;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox(
            "How to use:\n" +
            "- Place in the scene a gameObject with TowerGridMaker component\n" +
            "- Place it in the same Position as this object\n" +
            "- Edit freely the grid and then assign GridData to TowerGridManager",
            MessageType.Info);
    }
}
