using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StandardButtonAnimator))]
[CanEditMultipleObjects]
public class StandardButtonAnimatorInspector : Editor
{

    private SerializedProperty m_IdleProp;
    private SerializedProperty m_OnDownProp;
    private SerializedProperty m_OnUpProp;
    private SerializedProperty m_PressedProp;

    private SerializedProperty m_IdleSpeedProp;
    private SerializedProperty m_OnDownSpeedProp;
    private SerializedProperty m_OnUpSpeedProp;
    private SerializedProperty m_PressedSpeedProp;

    void OnEnable()
    {
        m_IdleProp = serializedObject.FindProperty("IdleAnimation");
        m_OnDownProp = serializedObject.FindProperty("OnDownAnimation");
        m_PressedProp = serializedObject.FindProperty("PressedAnimation");
        m_OnUpProp = serializedObject.FindProperty("OnUpAnimation");

        m_IdleSpeedProp = serializedObject.FindProperty("IdleAnimationTimeScale");
        m_OnDownSpeedProp = serializedObject.FindProperty("OnDownAnimationTimeScale");
        m_PressedSpeedProp = serializedObject.FindProperty("PressedAnimationTimeScale");
        m_OnUpSpeedProp = serializedObject.FindProperty("OnUpAnimationTimeScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_IdleProp);
        EditorGUILayout.PropertyField(m_OnDownProp);
        EditorGUILayout.PropertyField(m_PressedProp);
        EditorGUILayout.PropertyField(m_OnUpProp);

        if (m_IdleProp.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(m_IdleSpeedProp);
        }

        if (m_OnDownProp.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(m_OnDownSpeedProp);
        }

        if (m_PressedProp.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(m_PressedSpeedProp);
        }

        if (m_OnUpProp.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(m_OnUpSpeedProp);
        }

        if (GUILayout.Button("Apply animations to Animation component"))
        {
            ApplyAnimationsToComponent();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ApplyAnimationsToComponent()
    {
        var buttonAnimator = target as StandardButtonAnimator;
        var animation = buttonAnimator.GetComponent<Animation>();
        var clipNames = new List<string>();
        foreach (AnimationState state in animation)
        {
            clipNames.Add(state.clip.name);
        }

        animation.clip = null;
        foreach (var clipName in clipNames)
        {
            animation.RemoveClip(clipName);
        }

        if (buttonAnimator.IdleAnimation)
        {
            animation.AddClip(buttonAnimator.IdleAnimation, buttonAnimator.IdleAnimation.name);
        }

        if (buttonAnimator.OnDownAnimation)
        {
            animation.AddClip(buttonAnimator.OnDownAnimation, buttonAnimator.OnDownAnimation.name);
        }

        if (buttonAnimator.PressedAnimation)
        {
            animation.AddClip(buttonAnimator.PressedAnimation, buttonAnimator.PressedAnimation.name);
        }

        if (buttonAnimator.OnUpAnimation)
        {
            animation.AddClip(buttonAnimator.OnUpAnimation, buttonAnimator.OnUpAnimation.name);
        }
    }
}
