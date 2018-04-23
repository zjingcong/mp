// grid map gen ui: set heuristic function and build

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridMapGen))]
[CanEditMultipleObjects]
public class GridMapGenEditor : Editor {

    public string[] options = new string[] { "Euclidean", "Manhattan", "Diagonal" };
    SerializedProperty heuProp;

    private void OnEnable()
    {
        heuProp = serializedObject.FindProperty("heuIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        GridMapGen mapGen = (GridMapGen)target;
        heuProp.intValue = EditorGUILayout.Popup("Heuristic Function", heuProp.intValue, options);

        if (GUILayout.Button("Build"))
        {
            mapGen.Scan();  // scan height to generate grid map
            mapGen.heuIndex = heuProp.intValue;    // set grid map heuristic function
            Debug.Log("Select Heuristic Function ID: " + mapGen.heuIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
# endif
