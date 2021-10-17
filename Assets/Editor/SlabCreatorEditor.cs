using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SlabCreator))]
public class SlabCreatorEditor : Editor
{
    SlabCreator currentSlabCreator;

    private void OnEnable()
    {
        currentSlabCreator = (SlabCreator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Create Rect Faces"))
        {
            currentSlabCreator.GenerateRectFaces();
        }

        if (GUILayout.Button("Delete Slabs"))
        {
            currentSlabCreator.DeleteSlabs();
        }
    }
}