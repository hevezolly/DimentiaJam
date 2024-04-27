using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using Audio;

public class GlobalSoundSettings : EditorWindow {

    [MenuItem("Tools/SoundSettings")]
    private static void ShowWindow() {
        var window = GetWindow<GlobalSoundSettings>();
        window.titleContent = new GUIContent("GlobalSoundSettings");
        window.Show();
    }
    [SerializeField] private AllSoundGroups allSoundGroups;

    private Vector2 scrollPosition;
    private Dictionary<SoundGroup, int> hasListeners;

    private void OnEnable() 
    {
        hasListeners = new Dictionary<SoundGroup, int>();
        var groups = AssetDatabase.FindAssets($"t:{nameof(SoundGroup)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<SoundGroup>).OrderBy(g => g.order).ToList();
        
        allSoundGroups.groups = groups;

        foreach (var g in AssetDatabase.FindAssets($"t:{nameof(SoundPreset)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<SoundPreset>)
            .Select(g => g.group)
            .Where(g => g != null)) 
        {
            if (!hasListeners.ContainsKey(g))
                hasListeners[g] = 0;
            hasListeners[g]++;
        }
    }

    private const float ButtonWidht = 50 + Space;
    private const float ButtonMinScale = 1f/10f;
    private const float Space = 5;

    private const string GroupsLocation = "Assets/Audio/Settings";
    private const string InitialName = "Sound Group.asset";

    private void OnGUI() {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        var ToDelete = new List<SoundGroup>();
        foreach (var g in allSoundGroups.groups) 
        {
            EditorGUILayout.BeginHorizontal(); 

                var count = hasListeners.GetValueOrDefault(g, 0);
                EditorGUILayout.LabelField($"presets: {count}");
                var newName = EditorGUILayout.TextField(g.name);
                EditorGUI.BeginDisabledGroup(count > 0);
                if (GUILayout.Button("Delete")) 
                {
                    ToDelete.Add(g);
                }
                EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();
            if (newName != g.name) 
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(g), newName);
                g.name = newName;
            }
            var e = Editor.CreateEditor(g);
            e.OnInspectorGUI();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();
        foreach (var g in ToDelete) 
        {
            allSoundGroups.groups.Remove(g);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(g));
        }
        if (GUILayout.Button("Add")) 
        {
            var unityPath = GroupsLocation + "/" + InitialName;
            var group = ScriptableObject.CreateInstance<SoundGroup>();
            group.order = allSoundGroups.groups.Count;
            AssetDatabase.CreateAsset(group, AssetDatabase.GenerateUniqueAssetPath(unityPath));
            allSoundGroups.groups.Add(group);
        }
    }
}
