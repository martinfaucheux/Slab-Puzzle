using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraControler))]
public class CameraControlerScript : Editor
{
    CameraControler cameraControler;

    private void OnEnable()
    {
        cameraControler = (CameraControler)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		if(GUILayout.Button("Smooth Zooming"))
		{
            cameraControler.TriggerSmoothZooming(true);

        }
    }
}