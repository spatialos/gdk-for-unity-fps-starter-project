using Improbable.Gdk.Guns;
using UnityEditor;
using UnityEngine;

namespace Fps.GunPickups
{
    [CustomPropertyDrawer(typeof(SpatialGunDataProperty))]
    public class SpatialGunEditor : PropertyDrawer
    {
        private GunDictionary gunDict;
        private string[] gunStrings;

        private const int gunButtonsPerRow = 3;
        private const int gunButtonHeight = 15;

        private static float lastHeight = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RefreshGunDict();

            var lineRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Entry 1 - Gun Dictionary
            var gunDictionary = property.FindPropertyRelative("Guns");

            EditorGUI.PropertyField(lineRect, gunDictionary);
            lineRect.y += EditorGUIUtility.singleLineHeight;

            // Spacer
            lineRect.y += EditorGUIUtility.standardVerticalSpacing;

            // Entry 2 - Weapon buttons
            if (gunDict)
            {
                lineRect.height = WeaponSelectionContentHeight();
                var idProperty = property.FindPropertyRelative("GunId");
                var selectedGunId = idProperty.intValue;

                selectedGunId = GUI.SelectionGrid(lineRect, selectedGunId, gunStrings, 3);
                lineRect.y += lineRect.height;


                if (selectedGunId >= 0)
                {
                    idProperty.intValue = selectedGunId;
                }
                else
                {
                    lineRect.height = EditorGUIUtility.singleLineHeight * 2;
                    EditorGUI.HelpBox(lineRect, "Warning: Multiple gun types selected", MessageType.Warning);
                    lineRect.y += EditorGUIUtility.singleLineHeight * 2;
                }
            }

            lastHeight = lineRect.y - position.y;
        }

        private string GetGunName(int index)
        {
            if (index >= GunDictionary.Count)
            {
                return "INVALID";
            }

            return GunDictionary.Get(index).name;
        }

        private float WeaponSelectionContentHeight()
        {
            return gunDict == null
                ? EditorGUIUtility.singleLineHeight
                : gunButtonHeight * (GunDictionary.Count / gunButtonsPerRow + 1);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RefreshGunDict();
            return lastHeight <= 0 ? EditorGUI.GetPropertyHeight(property) : lastHeight;
        }

        private bool RefreshGunDict()
        {
            var dirty = false;
            var propGunDict = GunDictionary.Instance;

            if (gunDict != propGunDict)
            {
                dirty = true;
                gunDict = propGunDict;
            }

            if (dirty && gunDict)
            {
                gunStrings = new string[GunDictionary.Count];
                for (var i = 0; i < GunDictionary.Count; i++)
                {
                    gunStrings[i] = GetGunName(i);
                }
            }

            return dirty;
        }
    }
}
