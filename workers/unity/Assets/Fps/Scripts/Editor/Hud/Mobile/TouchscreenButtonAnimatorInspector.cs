using System.Collections.Generic;
using Fps.UI;
using UnityEditor;
using UnityEngine;

namespace Fps.Editor
{
    [CustomEditor(typeof(TouchscreenButtonAnimator))]
    [CanEditMultipleObjects]
    public class TouchscreenButtonAnimatorInspector : UnityEditor.Editor
    {
        private SerializedProperty idleProp;
        private SerializedProperty onDownProp;
        private SerializedProperty onUpProp;
        private SerializedProperty pressedProp;

        private SerializedProperty idleSpeedProp;
        private SerializedProperty onDownSpeedProp;
        private SerializedProperty onUpSpeedProp;
        private SerializedProperty pressedSpeedProp;

        private void OnEnable()
        {
            idleProp = serializedObject.FindProperty("IdleAnimation");
            onDownProp = serializedObject.FindProperty("OnDownAnimation");
            pressedProp = serializedObject.FindProperty("PressedAnimation");
            onUpProp = serializedObject.FindProperty("OnUpAnimation");

            idleSpeedProp = serializedObject.FindProperty("IdleAnimationTimeScale");
            onDownSpeedProp = serializedObject.FindProperty("OnDownAnimationTimeScale");
            pressedSpeedProp = serializedObject.FindProperty("PressedAnimationTimeScale");
            onUpSpeedProp = serializedObject.FindProperty("OnUpAnimationTimeScale");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject,
                idleProp.name,
                onDownProp.name,
                pressedProp.name,
                onUpProp.name,
                idleSpeedProp.name,
                onDownSpeedProp.name,
                pressedSpeedProp.name,
                onUpSpeedProp.name);

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(idleProp);
            if (idleProp.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(idleSpeedProp);
            }

            EditorGUILayout.PropertyField(onDownProp);
            if (onDownProp.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(onDownSpeedProp);
            }

            EditorGUILayout.PropertyField(pressedProp);
            if (pressedProp.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(pressedSpeedProp);
            }

            EditorGUILayout.PropertyField(onUpProp);
            if (onUpProp.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(onUpSpeedProp);
            }

            if (GUILayout.Button("Apply animations to Animation component"))
            {
                ApplyAnimationsToComponent();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ApplyAnimationsToComponent()
        {
            var buttonAnimator = target as TouchscreenButtonAnimator;
            var animation = buttonAnimator.GetComponent<UnityEngine.Animation>();
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
}
