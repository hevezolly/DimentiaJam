using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(ScriptableValueField<>))]
public class ScrptableValuePropertyDrawer : PropertyDrawer
{
    private const string ScriptableFieldName = "scriptableValue";
    private const string PropertyFieldName = "currentValue";
    private const string UseSoFieldName = "UseSO";
    private const string NoSoValueFiledName = "noSOValue";
    private const float Spacing = 2;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (UseSoProp(property).boolValue) 
        {
            var scriptableProperty = SOProperty(property);
            var height = EditorGUI.GetPropertyHeight(scriptableProperty);
            if (scriptableProperty.objectReferenceValue != null)
                height += Spacing * 2 + EditorGUI.GetPropertyHeight(SoValueProperty(property));
            return height;
        }
        else {
            return EditorGUI.GetPropertyHeight(NoSoProperty(property));
        }
    }

    private SerializedProperty SoValueProperty(SerializedProperty property) =>
        new SerializedObject(SOProperty(property).objectReferenceValue).FindProperty(PropertyFieldName);

    private SerializedProperty SOProperty(SerializedProperty property) => 
        property.FindPropertyRelative(ScriptableFieldName);

    private SerializedProperty NoSoProperty(SerializedProperty property) => 
        property.FindPropertyRelative(NoSoValueFiledName);

    private SerializedProperty UseSoProp(SerializedProperty property) => 
        property.FindPropertyRelative(UseSoFieldName);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var useSoProp = UseSoProp(property);

        var boolRect = new Rect(position.x, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

        var l = label.text;
        var newUseSo = EditorGUI.Toggle(boolRect, new GUIContent(l), useSoProp.boolValue);
        useSoProp.boolValue = newUseSo;

        var bellow_label = new StringBuilder();
        for (var i = 0; i < label.text.Length; i++)
        {
            bellow_label.Append(" ");
        }
        var space_label = new GUIContent(bellow_label.ToString());

        if (newUseSo) 
        {
            var scriptableObjectProperty = SOProperty(property);
            var refHeight = EditorGUI.GetPropertyHeight(scriptableObjectProperty);
            var base_rect = new Rect(position.x, position.y, position.width, refHeight);
            base_rect.x += EditorGUIUtility.singleLineHeight + Spacing;
            base_rect.width -= EditorGUIUtility.singleLineHeight + Spacing;
            if (scriptableObjectProperty.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(base_rect, scriptableObjectProperty, space_label);
                EditorGUI.EndProperty();
                return;
            }

            var parameter_rect = new Rect(position.x, base_rect.yMax + Spacing, position.width, position.height - refHeight - Spacing);
            var serializedObj = new SerializedObject(scriptableObjectProperty.objectReferenceValue);
            var parameter = serializedObj.FindProperty(PropertyFieldName);


            EditorGUI.PropertyField(base_rect, scriptableObjectProperty, space_label);

            EditorGUI.PropertyField(parameter_rect, parameter, space_label);

            serializedObj.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        } 
        else 
        {
            var parameter = NoSoProperty(property);

            var base_rect = position;
            base_rect.x += EditorGUIUtility.singleLineHeight + Spacing;
            base_rect.width -= EditorGUIUtility.singleLineHeight + Spacing;

            EditorGUI.PropertyField(base_rect, parameter, space_label);
            EditorGUI.EndProperty();
        }
    }
}