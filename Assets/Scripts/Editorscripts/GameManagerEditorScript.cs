using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameManager))]
public class GameManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (GameManager)target;
        GUILayout.TextArea("Does not work in editor lmao");
        GUILayout.TextArea("Current world: " + script.GetWorld());

        if(GUILayout.Button("Switch to world 1", GUILayout.Height(40)))
        {
            script.SetWorld(World.World1);
        }
        if(GUILayout.Button("Switch to world 2", GUILayout.Height(40)))
        {
            script.SetWorld(World.World1);
        }
        if(GUILayout.Button("Switch to world 3", GUILayout.Height(40)))
        {
            script.SetWorld(World.World1);
        }
        
    }
}
