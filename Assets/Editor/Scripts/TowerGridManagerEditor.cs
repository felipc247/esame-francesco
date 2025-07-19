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
            "Piazza nella scene un GameObject con script TowerGridMaker:\n" +
            "- Mettilo nella stessa posizione di questo oggetto\n" +
            "- Edita la griglia a piacere e poi assegna GridData a TowerGridManager",
            MessageType.Info);
    }
}
