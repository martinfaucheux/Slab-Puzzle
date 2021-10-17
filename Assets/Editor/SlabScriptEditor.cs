using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PermutableSlab))]
public class SlabScriptEditor : Editor
{
    Slab currentSlab;

    private void OnEnable()
    {
        currentSlab = (Slab)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		if(GUILayout.Button("Init Neighbourhood"))
		{
            currentSlab.InitNeighborhood();
		}
    }
}