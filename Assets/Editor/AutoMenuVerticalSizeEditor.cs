using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoMenuVerticalSize))]
public class AutoMenuVerticalSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Adjust Size"))
        {
            ((AutoMenuVerticalSize)target).AdjustSize();
        }
    }
}
