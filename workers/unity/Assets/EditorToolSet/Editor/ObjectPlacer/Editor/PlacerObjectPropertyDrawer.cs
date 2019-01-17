using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(PlacerObjectSet))]
public class PlacerObjectPropertyDrawer : PropertyDrawer
{
    private float LINE = 18f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var defaultLabelWidth = EditorGUIUtility.labelWidth;
        var defaultFieldWidth = EditorGUIUtility.fieldWidth;
        var toggleOptionLabelWidth = defaultLabelWidth - 20;
        var minMaxLabelWidth = 30;
        var resetButtonX = 180 + defaultLabelWidth;
        var resetButtonWidth = 60;
        Rect subPos;
        Rect checkBoxPos;

        // List<GameObject> Objects
        var Objects = property.FindPropertyRelative("Objects");
        EditorGUI.PropertyField(position, Objects, true);
        position.y += LINE;
        if (Objects.isExpanded)
        {
            position.y += LINE * (Objects.arraySize + 1);
        }

        // bool AlignToNormal
        var AlignToNormal = property.FindPropertyRelative("AlignToNormal");
        EditorGUI.LabelField(position, "Align To Normal");
        checkBoxPos = position;
        checkBoxPos.x += toggleOptionLabelWidth;
        checkBoxPos.width = 20;
        AlignToNormal.boolValue = EditorGUI.Toggle(checkBoxPos, AlignToNormal.boolValue);

        if (AlignToNormal.boolValue)
        {
            var AlignMaxAngle = property.FindPropertyRelative("AlignMaxAngle");
            subPos = position;
            subPos.x += defaultLabelWidth;
            subPos.width = minMaxLabelWidth * 2.5f + EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = minMaxLabelWidth * 2.5f;
            EditorGUI.PropertyField(subPos, AlignMaxAngle, new GUIContent("Max Angle"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            subPos.x = resetButtonX;
            subPos.width = resetButtonWidth;
            if (GUI.Button(subPos, "RESET"))
            {
                AlignMaxAngle.floatValue = 180f;
            }
        }
        position.y += LINE;

        // bool RandomYaw
        var RandomYaw = property.FindPropertyRelative("RandomYaw");
        EditorGUI.LabelField(position, "Random Yaw");
        checkBoxPos = position;
        checkBoxPos.x += toggleOptionLabelWidth;
        checkBoxPos.width = 20;
        RandomYaw.boolValue = EditorGUI.Toggle(checkBoxPos, RandomYaw.boolValue);
        if (RandomYaw.boolValue)
        {
            var YawMin = property.FindPropertyRelative("YawMin");
            var YawMax = property.FindPropertyRelative("YawMax");
            subPos = position;
            subPos.x += defaultLabelWidth;
            subPos.width = minMaxLabelWidth + EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = minMaxLabelWidth;
            EditorGUI.PropertyField(subPos, YawMin, new GUIContent("Min"));
            subPos.x += subPos.width + 5;
            EditorGUI.PropertyField(subPos, YawMax, new GUIContent("Max"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            subPos.x = resetButtonX;
            subPos.width = resetButtonWidth;
            if (GUI.Button(subPos, "RESET"))
            {
                YawMin.floatValue = 0f;
                YawMax.floatValue = 360f;
            }
        }
        position.y += LINE;

        // float RandomPitchValue
        var RandomPitch = property.FindPropertyRelative("RandomPitch");
        EditorGUI.LabelField(position, "Random Pitch");
        checkBoxPos = position;
        checkBoxPos.x += toggleOptionLabelWidth;
        checkBoxPos.width = 20;
        RandomPitch.boolValue = EditorGUI.Toggle(checkBoxPos, RandomPitch.boolValue);
        if (RandomPitch.boolValue)
        {
            var RandomPitchValue = property.FindPropertyRelative("RandomPitchValue");
            subPos = position;
            subPos.x += defaultLabelWidth;
            subPos.width = minMaxLabelWidth + EditorGUIUtility.fieldWidth;
            subPos.x += subPos.width + 5;
            EditorGUIUtility.labelWidth = minMaxLabelWidth;
            EditorGUI.PropertyField(subPos, RandomPitchValue, new GUIContent("Max"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            subPos.x = resetButtonX;
            subPos.width = resetButtonWidth;
            if (GUI.Button(subPos, "RESET"))
            {
                RandomPitchValue.floatValue = 0f;
            }
        }
        position.y += LINE;

        var LimitSlope = property.FindPropertyRelative("LimitSlope");
        EditorGUI.LabelField(position, "Limit Slope");
        checkBoxPos = position;
        checkBoxPos.x += toggleOptionLabelWidth;
        checkBoxPos.width = 20;
        LimitSlope.boolValue = EditorGUI.Toggle(checkBoxPos, LimitSlope.boolValue);
        if (LimitSlope.boolValue)
        {
            // float ValidSlopeMin
            // float ValidSlopeMax
            var ValidSlopeMin = property.FindPropertyRelative("ValidSlopeMin");
            var ValidSlopeMax = property.FindPropertyRelative("ValidSlopeMax");
            subPos = position;
            subPos.x += defaultLabelWidth;
            subPos.width = minMaxLabelWidth + EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = minMaxLabelWidth;
            EditorGUI.PropertyField(subPos, ValidSlopeMin, new GUIContent("Min"));
            subPos.x += subPos.width + 5;
            EditorGUI.PropertyField(subPos, ValidSlopeMax, new GUIContent("Max"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            subPos.x = resetButtonX;
            subPos.width = resetButtonWidth;
            if (GUI.Button(subPos, "RESET"))
            {
                ValidSlopeMin.floatValue = 0f;
                ValidSlopeMax.floatValue = 180f;
            }
        }
        position.y += LINE;

        // bool ApplyScale
        // float ScaleMin
        // float ScaleMax
        var ApplyScale = property.FindPropertyRelative("ApplyScale");
        EditorGUI.LabelField(position, "Apply Scale");
        checkBoxPos = position;
        checkBoxPos.x += toggleOptionLabelWidth;
        checkBoxPos.width = 20;
        ApplyScale.boolValue = EditorGUI.Toggle(checkBoxPos, ApplyScale.boolValue);
        if (ApplyScale.boolValue)
        {
            var ScaleMin = property.FindPropertyRelative("ScaleMin");
            var ScaleMax = property.FindPropertyRelative("ScaleMax");
            subPos = position;
            subPos.x += defaultLabelWidth;
            subPos.width = minMaxLabelWidth + EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = minMaxLabelWidth;
            EditorGUI.PropertyField(subPos, ScaleMin, new GUIContent("Min"));
            subPos.x += subPos.width + 5;
            EditorGUI.PropertyField(subPos, ScaleMax, new GUIContent("Max"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            subPos.x = resetButtonX;
            subPos.width = resetButtonWidth;
            if (GUI.Button(subPos, "RESET"))
            {
                ScaleMin.floatValue = 1f;
                ScaleMax.floatValue = 1f;
            }
        }
        position.y += LINE;

        var ApplyZOffset = property.FindPropertyRelative("ApplyZOffset");
        EditorGUI.LabelField(position, "Apply Z Offset");
        checkBoxPos = position;
        checkBoxPos.x += toggleOptionLabelWidth;
        checkBoxPos.width = 20;
        ApplyZOffset.boolValue = EditorGUI.Toggle(checkBoxPos, ApplyZOffset.boolValue);
        if (ApplyZOffset.boolValue)
        {
            var MinZOffset = property.FindPropertyRelative("MinZOffset");
            var MaxZOffset = property.FindPropertyRelative("MaxZOffset");
            subPos = position;
            subPos.x += defaultLabelWidth;
            subPos.width = minMaxLabelWidth + EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = minMaxLabelWidth;
            EditorGUI.PropertyField(subPos, MinZOffset, new GUIContent("Min"));
            subPos.x += subPos.width + 5;
            EditorGUI.PropertyField(subPos, MaxZOffset, new GUIContent("Max"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            subPos.x = resetButtonX;
            subPos.width = resetButtonWidth;
            if (GUI.Button(subPos, "RESET"))
            {
                MinZOffset.floatValue = 0f;
                MaxZOffset.floatValue = 0f;
            }
        }
        position.y += LINE;

    }
}
