using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CastRay))]
public class TestRayScript : Editor
{
    CastRay castRay;

    private void OnEnable()
    {
        castRay = (CastRay)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		if(GUILayout.Button("Cast Ray"))
		{
            castRay.Touch();

        }
    }
}