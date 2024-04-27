using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;
using System;
using Audio;

[CustomPropertyDrawer(typeof(Sound))]
public class SoundEditor: PropertyDrawer {

    private const float buttonRatio = 4f/5f;
    private const float space = 5f;

    private float line => EditorGUIUtility.singleLineHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        var widht = position.width;
        var buttonPos = new Rect(position.x + widht * buttonRatio - space, position.y, 
            widht * (1 - buttonRatio), line);
        var fieldPos = new Rect(position.x, position.y, widht * buttonRatio - space, line);

        var foldoutRect = new Rect(buttonPos.position.x + buttonPos.width + space, position.y, line, line);

        var presetField = property.FindPropertyRelative(nameof(Sound.preset));

        EditorGUI.BeginChangeCheck();

        var oldPreset = presetField.objectReferenceValue as SoundPreset;

        EditorGUI.PropertyField(fieldPos, presetField, label);

        


        if (EditorGUI.EndChangeCheck()) 
        {
            property.serializedObject.ApplyModifiedProperties();
            var preset = presetField.objectReferenceValue as SoundPreset;
            oldPreset = preset;
        }

        if (GUI.Button(buttonPos, "find")) 
        {
            var menu = new GenericMenu();

            foreach (var option in AssetDatabase.FindAssets($"t:{nameof(PlayableSoundSettings)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(p => AssetDatabase.LoadAssetAtPath<PlayableSoundSettings>(p))
                .Where(p => p.IsReady())) 
            {
                var name = new StringBuilder();
                if (option.Group() != null) 
                    name.Append($"{option.Group().name}/");
                if (!string.IsNullOrEmpty(option.Preffix())) 
                    name.Append($"{option.Preffix()}/");
                name.Append(option.name);
                var current = option;
                menu.AddItem(new GUIContent(name.ToString()), false, () => 
                {
                    presetField.objectReferenceValue = current;
                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();
                });
            }
            menu.ShowAsContext();
        }

        // EditorGUI.BeginChangeCheck();

       

        // var isExpanded = overrideSettings.boolValue;
        
        // EditorGUI.BeginDisabledGroup(oldPreset == null);
        // var newOpen = EditorGUI.Toggle(foldoutRect, isExpanded);
        // EditorGUI.EndDisabledGroup();
        // if (oldPreset == null)
        //     newOpen = false;

        // if (newOpen && newOpen != isExpanded) 
        // {
        //     settingsField.SetValue(oldPreset.defaultSettings.Copy());
        //     property.serializedObject.ApplyModifiedProperties();
        //     property.serializedObject.Update();
        // }
        
        // isExpanded = newOpen;
        // overrideSettings.boolValue = isExpanded;

        
        // EditorGUI.indentLevel++;
        // if (isExpanded) 
        // {
        //     var dots = new String('-', 50);
        //     EditorGUI.LabelField(new Rect(position.x, position.y + line * 1.5f, position.width, line), 
        //         $"{dots}OVERRIDE{dots}", new GUIStyle(EditorStyles.boldLabel) 
        //         {
        //             alignment = TextAnchor.MiddleCenter
        //         });
        //     EditorGUI.BeginDisabledGroup(true);
        //     var preset = presetField.objectReferenceValue as SoundPreset;

        //     if (preset != null && preset.clip != null)
        //         EditorGUI.ObjectField(new Rect(position.x, position.y + line * 3, position.width, line), 
        //             new GUIContent("clip"), preset.clip, typeof(AudioClip), false);
        //     EditorGUI.EndDisabledGroup();

        //     SoundSettingsDrawer.DrawPropertyInline(settingsField, position.x, position.y + line * 4, position.width);
        // }
        // EditorGUI.indentLevel--;

        // if (EditorGUI.EndChangeCheck()) 
        // {
        //     property.serializedObject.ApplyModifiedProperties();
        //     EditorUtility.SetDirty(property.serializedObject.targetObject);
        // }
    }
}
