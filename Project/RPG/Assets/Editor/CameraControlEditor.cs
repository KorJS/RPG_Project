using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraControl))]
public class CameraControlEditor : Editor
{
    private CameraControl cameraControl = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        cameraControl = (CameraControl)target;

        EditorGUILayout.LabelField("- Camera Helper -");

        if (GUILayout.Button("Save camera's position now"))
        {
            Camera cam = Camera.main;

            if (cam)
            {
                Vector3 camPos = cam.transform.localPosition;
                cameraControl.cameraSettings.camPositionOffset = camPos;
            }
        }
    }
}
