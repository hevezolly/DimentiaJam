using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Audio;

public static class SoundSettingsDrawer
{
    public static int GetLineCount(SerializedProperty property) 
    {
        var val = Iterate(property).Count();
        var loopType = (SoundRepeatType)property.FindPropertyRelative(nameof(SoundPlaySettings.repeat)).intValue;
        if (loopType == SoundRepeatType.Number)
            val++;
        return val;
    }

    private static IEnumerable<SerializedProperty> Iterate(SerializedProperty soundProperty) 
    {
        var iterator = soundProperty.Copy();
        var check = soundProperty.Copy();
        var hasMore = check.NextVisible(false);
        var end = soundProperty.GetEndProperty();
        iterator.NextVisible(true);
        do 
        {
            if (hasMore && iterator.propertyPath == check.propertyPath)
                break;
            yield return iterator;
        }
        while (iterator.NextVisible(false));
    }   

    public static void DrawSoundSettingsInline(SerializedProperty soundProperty) 
    {
        foreach (var property in Iterate(soundProperty)) 
            EditorGUILayout.PropertyField(property);


        var loopType = (SoundRepeatType)soundProperty.FindPropertyRelative(nameof(SoundPlaySettings.repeat)).intValue;
        if (loopType == SoundRepeatType.Number) 
        {
            var num = soundProperty.FindPropertyRelative(nameof(SoundPlaySettings.numberOfRepeats));
            EditorGUILayout.PropertyField(num);
        }

        var pitchType = (PitchType)soundProperty.FindPropertyRelative(nameof(SoundPlaySettings.pitchType)).intValue;
        if (pitchType == PitchType.Constant) 
            EditorGUILayout.PropertyField(soundProperty.FindPropertyRelative(nameof(SoundPlaySettings.constantPitch)));            
        else
            EditorGUILayout.PropertyField(soundProperty.FindPropertyRelative(nameof(SoundPlaySettings.minMaxPitch)));
    }
}
