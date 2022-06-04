using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Screencapture))]
public class ScreencaptureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Screencapture screencapture = (Screencapture)target;
        if (GUILayout.Button("Take Screenshot"))
        {
            screencapture.SaveScreenshot();
        }
    }

}