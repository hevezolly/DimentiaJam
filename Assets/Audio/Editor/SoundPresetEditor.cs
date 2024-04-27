using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using Audio;

[CustomEditor(typeof(SoundPreset))]
public class SoundPresetEditor : Editor {

    private string[] names;
    private string[] groupVariants;

    private void OnEnable() {
        groupVariants = AssetDatabase.FindAssets($"t:{nameof(SoundGroup)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .OrderBy(p => p).ToArray();
        names = groupVariants.Select(v => Path.GetFileNameWithoutExtension(v)).ToArray();
    }

    public override void OnInspectorGUI() {
        
        
        var sound = (SoundPreset)target;
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SoundPreset.Hide)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SoundPreset.clip)));

        var index = 0;

        if (sound.group == null) 
        {
            sound.group = AssetDatabase.LoadAssetAtPath<SoundGroup>(groupVariants[0]);
        }
        else 
        {
            index = Array.IndexOf(names, sound.group.name);
            if (index < 0) 
            {
                index = 0;
                sound.group = AssetDatabase.LoadAssetAtPath<SoundGroup>(groupVariants[0]);
            }
        }

        GUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Sound type");
        var newIndex = EditorGUILayout.Popup(index, names);

        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SoundPreset.prefix)));

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        var line = new String('-', 50);

        EditorGUILayout.LabelField($"{line}Settings{line}", new GUIStyle(EditorStyles.boldLabel) 
        {
            alignment = TextAnchor.MiddleCenter
        }, GUILayout.ExpandWidth(true)); 

        if (index != newIndex) 
        {
            sound.group = AssetDatabase.LoadAssetAtPath<SoundGroup>(groupVariants[newIndex]);
        }

        var iterator = serializedObject.FindProperty(nameof(SoundPreset.defaultSettings));
        SoundSettingsDrawer.DrawSoundSettingsInline(iterator);


        if (EditorGUI.EndChangeCheck()) 
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }

    [MenuItem("Assets/To Sound Preset")]
    public static void MakeSoundPreset() 
    {
        var iteration = Selection.assetGUIDs
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(p => (AssetDatabase.LoadAssetAtPath<AudioClip>(p), p))
            .Where(v => v.Item1 != null).ToArray();
        var isSingle = iteration.Length == 1;
        foreach (var (clip, path) in iteration) 
        {
            var inst = ScriptableObject.CreateInstance<SoundPreset>();
            inst.clip = clip;
            var newPath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(Path.GetDirectoryName(path), clip.name+".asset"));
            EditorUtility.SetDirty(inst);
            if (isSingle)
                ProjectWindowUtil.CreateAsset(inst, newPath);
            else 
                AssetDatabase.CreateAsset(inst, newPath);
        }
    }

    [MenuItem("Assets/To Sound Preset", true)]
    public static bool MakeSoundPresetValidate() 
    {
        return Selection.assetGUIDs
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<AudioClip>)
            .Where(v => v != null).Any();
    }
}
