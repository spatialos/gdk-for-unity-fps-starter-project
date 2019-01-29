using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UITable))]
public class UITableTest : Editor
{
    private float f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
       return;
        for (var i = 0; i < styles.Length; i++)
        {
            var slider = styles[i];
            GUILayout.Label(slider);
            for (var j = 0; j < styles.Length; j++)
            {
                var thumb = styles[j];

                GUILayout.BeginHorizontal();
                GUILayout.Label(thumb);

                f = GUILayout.HorizontalSlider(f, 0, 1, slider, thumb);
                GUILayout.EndHorizontal();
            }
        }
    }

    private string[] styles =
    {
        "ColorPickerBox",
        "In BigTitle",
        "miniLabel",
        "LargeLabel",
        "BoldLabel",
        "MiniBoldLabel",
        "WordWrappedLabel",
        "WordWrappedMiniLabel",
        "WhiteLabel",
        "WhiteMiniLabel",
        "WhiteLargeLabel",
        "WhiteBoldLabel",
        "MiniTextField",
        "Radio",
        "miniButton",
        "miniButtonLeft",
        "miniButtonMid",
        "miniButtonRight",
        "toolbar",
        "toolbarbutton",
        "toolbarPopup",
        "toolbarDropDown",
        "toolbarTextField",
        "ToolbarSeachTextField",
        "ToolbarSeachTextFieldPopup",
        "ToolbarSeachCancelButton",
        "ToolbarSeachCancelButtonEmpty",
        "SearchTextField",
        "SearchCancelButton",
        "SearchCancelButtonEmpty",
        "HelpBox",
        "AssetLabel",
        "AssetLabel Partial",
        "AssetLabel Icon",
        "selectionRect",
        "MinMaxHorizontalSliderThumb",
        "DropDownButton",
        "BoldLabel",
        "Label",
        "MiniLabel",
        "MiniBoldLabel",
        "ProgressBarBack",
        "ProgressBarBar",
        "ProgressBarText",
        "FoldoutPreDrop",
        "IN Title",
        "IN TitleText",
        "BoldToggle",
        "Tooltip",
        "NotificationText",
        "NotificationBackground",
        "MiniPopup",
        "textField",
        "ControlLabel",
        "ObjectField",
        "ObjectFieldThumb",
        "ObjectFieldMiniThumb",
        "Toggle",
        "ToggleMixed",
        "ColorField",
        "Foldout",
        "TextFieldDropDown",
        "TextFieldDropDownText"
    };
}
