using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Audio;

[CustomEditor(typeof(SoundGroup))]
public class SoundGroupEditor : Editor {
    public override void OnInspectorGUI() {
        EditorGUI.BeginChangeCheck();
        var prop = serializedObject.GetIterator();
        prop.NextVisible(true);
        prop.NextVisible(false);
        do 
        {
            EditorGUILayout.PropertyField(prop);
        }
        while (prop.NextVisible(false));
        if (EditorGUI.EndChangeCheck()) 
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
