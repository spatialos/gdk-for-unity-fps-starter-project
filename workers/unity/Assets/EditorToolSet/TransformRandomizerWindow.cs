
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;


public class TransformRandomizerWindow : EditorWindow
{


    private float _positionXMin = 0;
    private float _positionXMax = 0;
    private float _positionYMin = 0;
    private float _positionYMax = 0;
    private float _positionZMin = 0;
    private float _positionZMax = 0;

    private float _scaleXMin = 1;
    private float _scaleXMax = 1;
    private float _scaleYMin = 1;
    private float _scaleYMax = 1;
    private float _scaleZMin = 1;
    private float _scaleZMax = 1;

    private float _rotationXMin = 0;
    private float _rotationXMax = 0;
    private float _rotationYMin = 0;
    private float _rotationYMax = 0;
    private float _rotationZMin = 0;
    private float _rotationZMax = 0;

    private enum TransformMode { Position, Scale, Rotation }
    private enum TransformOperation { Relative, Absolute }
    private TransformMode _currentTransformMode = TransformMode.Position;
    private TransformOperation _currentTransformOperation = TransformOperation.Relative;
    private Space _currentSpace = Space.World;
    private readonly string[] _transformModeOptions = { "Position", "Scale", "Rotation" };
    private readonly string[] _transformSpaceOptions = { "World", "Local" };
    private readonly string[] _transformOperationOptions = { "Relative", "Absolute" };

    private bool _linkScaling;

    private Rect guiRect = new Rect(0, 0, 0, 0);

    private GUIStyle labelStyle;

    private static TransformRandomizerWindow window;
    [MenuItem("ImprobaTools/Randomize Selected Object Transforms...")]
    public static void OpenWindow()
    {
        window = CreateInstance<TransformRandomizerWindow>();
        window.titleContent = new GUIContent("Randomizer");
        window.Show();
    }

    private void OnEnable()
    {
        labelStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("label");
    }

    private void OnDestroy()
    {
        Debug.Log("Destroyed TransformRandomizerWindow");
        window = null;
    }

    private int _reselectCapturedDataState = 0;
    private void OnSelectionChange()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if (_reselectCapturedDataState == 1)
        {
            _reselectCapturedDataState = 2;
            EditorTools.SelectList(_capturedData.Keys.ToList());
        }
        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope())
            {
                ToolBar_TransformMode();
                using (new GUILayout.HorizontalScope())
                {
                    Rect lastRect;
                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Label("");
                        if (_currentTransformMode == TransformMode.Scale && _linkScaling)
                        {
                            GUILayout.Label(" ");
                            GUILayout.Label(" ");
                            GUILayout.Label(" ");
                        }
                        else
                        {
                            Button_AxisLabel("X");
                            Button_AxisLabel("Y");
                            Button_AxisLabel("Z");

                        }
                    }

                    const int floatFieldWidth = 100;
                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Label("Min");

                        switch (_currentTransformMode)
                        {
                            case TransformMode.Position:
                                GUI.enabled = _xAxisActive;
                                _positionXMin =
                                    EditorGUILayout.FloatField(_positionXMin, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _yAxisActive;
                                _positionYMin =
                                    EditorGUILayout.FloatField(_positionYMin, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _zAxisActive;
                                _positionZMin =
                                    EditorGUILayout.FloatField(_positionZMin, GUILayout.MaxWidth(floatFieldWidth));
                                break;
                            case TransformMode.Scale:
                                GUI.enabled = _xAxisActive || _linkScaling;
                                _scaleXMin = EditorGUILayout.FloatField(_scaleXMin, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _yAxisActive && !_linkScaling;
                                _scaleYMin = EditorGUILayout.FloatField(_scaleYMin, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _zAxisActive && !_linkScaling;
                                _scaleZMin = EditorGUILayout.FloatField(_scaleZMin, GUILayout.MaxWidth(floatFieldWidth));

                                break;
                            case TransformMode.Rotation:
                                GUI.enabled = _xAxisActive;
                                _rotationXMin =
                                    EditorGUILayout.FloatField(_rotationXMin, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _yAxisActive;
                                _rotationYMin =
                                    EditorGUILayout.FloatField(_rotationYMin, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _zAxisActive;
                                _rotationZMin =
                                    EditorGUILayout.FloatField(_rotationZMin, GUILayout.MaxWidth(floatFieldWidth));
                                break;
                            default:
                                throw new System.ArgumentOutOfRangeException();
                        }
                        GUI.enabled = true;
                    }
                    using (new GUILayout.VerticalScope())
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Max");
                            if (_currentTransformMode == TransformMode.Scale)
                            {
                                _linkScaling = GUILayout.Toggle(_linkScaling, "Link");
                            }
                        }
                        switch (_currentTransformMode)
                        {
                            case TransformMode.Position:
                                GUI.enabled = _xAxisActive;
                                _positionXMax =
                                    EditorGUILayout.FloatField(_positionXMax, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _yAxisActive;
                                _positionYMax =
                                    EditorGUILayout.FloatField(_positionYMax, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _zAxisActive;
                                _positionZMax =
                                    EditorGUILayout.FloatField(_positionZMax, GUILayout.MaxWidth(floatFieldWidth));
                                break;
                            case TransformMode.Scale:
                                GUI.enabled = _xAxisActive || _linkScaling; ;
                                _scaleXMax = EditorGUILayout.FloatField(_scaleXMax, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _yAxisActive && !_linkScaling;
                                _scaleYMax = EditorGUILayout.FloatField(_scaleYMax, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _zAxisActive && !_linkScaling;
                                _scaleZMax = EditorGUILayout.FloatField(_scaleZMax, GUILayout.MaxWidth(floatFieldWidth));
                                break;
                            case TransformMode.Rotation:
                                GUI.enabled = _xAxisActive;
                                _rotationXMax =
                                    EditorGUILayout.FloatField(_rotationXMax, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _yAxisActive;
                                _rotationYMax =
                                    EditorGUILayout.FloatField(_rotationYMax, GUILayout.MaxWidth(floatFieldWidth));
                                GUI.enabled = _zAxisActive;
                                _rotationZMax =
                                    EditorGUILayout.FloatField(_rotationZMax, GUILayout.MaxWidth(floatFieldWidth));
                                break;
                            default:
                                throw new System.ArgumentOutOfRangeException();
                        }
                        GUI.enabled = true;
                        lastRect = GUILayoutUtility.GetLastRect();
                    }

                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Label("");
                        switch (_currentTransformMode)
                        {
                            case TransformMode.Position:
                                Button_ResetField(ref _positionXMin, ref _positionXMax, 0, lastRect);
                                Button_ResetField(ref _positionYMin, ref _positionYMax, 0, lastRect);
                                Button_ResetField(ref _positionZMin, ref _positionZMax, 0, lastRect);
                                break;
                            case TransformMode.Scale:
                                Button_ResetField(ref _scaleXMin, ref _scaleXMax, 1, lastRect);
                                Button_ResetField(ref _scaleYMin, ref _scaleYMax, 1, lastRect);
                                Button_ResetField(ref _scaleZMin, ref _scaleZMax, 1, lastRect);
                                break;
                            case TransformMode.Rotation:
                                Button_ResetField(ref _rotationXMin, ref _rotationXMax, 0, lastRect);
                                Button_ResetField(ref _rotationYMin, ref _rotationYMax, 0, lastRect);
                                Button_ResetField(ref _rotationZMin, ref _rotationZMax, 0, lastRect);
                                break;
                            default:
                                throw new System.ArgumentOutOfRangeException();
                        }
                    }
                }
                ToolBar_Space();
                ToolBar_AbsoRelaMode();
                EditorGUILayout.Space();
                Button_Randomize();
                using (new EditorGUILayout.HorizontalScope())
                {
                    Button_Close();
                    Button_Help();
                }
                EditorGUILayout.Space();
                GUILayout.Label(string.Format("Captured objects ({0})", _capturedData.Count));
                using (new EditorGUILayout.HorizontalScope())
                {
                    Button_CaptureState();
                    Button_ClearState();
                    Button_RestoreState();
                    Button_SelectState();
                }
            }


            if (guiRect.width < 10)
            {
                var rect = GUILayoutUtility.GetLastRect();
                if (rect.width > 10)
                {
                    guiRect = rect;
                }
            }
            else
            {
                if (window)
                {
                    //window.minSize = new Vector2(guiRect.x + guiRect.width, guiRect.y + guiRect.height + 10);
                    //window.maxSize = window.minSize;
                    window.minSize = new Vector2(10, guiRect.height + 10);
                    window.maxSize = new Vector2(guiRect.x + guiRect.width, guiRect.y + guiRect.height + 10);
                }
            }
            GUILayout.FlexibleSpace();

        }
    }

    private bool _xAxisActive;
    private bool _yAxisActive;
    private bool _zAxisActive;



    private void Button_AxisLabel(string s)
    {
        bool active;
        switch (s)
        {
            case "X":
                active = _xAxisActive;
                break;
            case "Y":
                active = _yAxisActive;
                break;
            case "Z":
                active = _zAxisActive;
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
        labelStyle.fontStyle = active ? FontStyle.Bold : FontStyle.Normal;
        labelStyle.normal.textColor = active ? Color.green : Color.green * .6f;
        if (GUILayout.Button(s, labelStyle))
        {
            switch (s)
            {
                case "X":
                    _xAxisActive = !_xAxisActive;
                    break;
                case "Y":
                    _yAxisActive = !_yAxisActive;
                    break;
                case "Z":
                    _zAxisActive = !_zAxisActive;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
        labelStyle.fontStyle = FontStyle.Normal;
        labelStyle.normal.textColor = Color.black;
    }

    private void ToolBar_TransformMode()
    {
        _currentTransformMode = (TransformMode)GUILayout.Toolbar((int)_currentTransformMode, _transformModeOptions);
    }

    private void ToolBar_Space()
    {
        GUI.enabled = _currentTransformMode != TransformMode.Scale;
        if (GUI.enabled)
        {
            _currentSpace = (Space)GUILayout.Toolbar((int)_currentSpace, _transformSpaceOptions);
        }
        else
        {
            GUILayout.Toolbar((int)Space.Self, _transformSpaceOptions);
        }
        GUI.enabled = true;
    }

    private void ToolBar_AbsoRelaMode()
    {
        _currentTransformOperation = (TransformOperation)GUILayout.Toolbar((int)_currentTransformOperation, _transformOperationOptions);
    }

    private void Button_ResetField(ref float fieldA, ref float fieldB, float defaultValue, Rect rect)
    {
        //if (GUI.Button(new Rect(rect.xMin + rect.width, rect.height * row, 16f, rect.height), "R"))
        GUI.enabled = FieldIsModified(fieldA, fieldB, defaultValue);
        var defaultColor = labelStyle.normal.textColor;
        if (GUI.enabled)
        {
            labelStyle.normal.textColor = Color.blue;
        }
        if (GUILayout.Button("Reset", labelStyle))
        {
            fieldA = defaultValue;
            fieldB = defaultValue;

            GUI.SetNextControlName("");
            GUI.FocusControl("");
        }
        labelStyle.normal.textColor = defaultColor;
        GUI.enabled = true;
    }

    private void Button_Close()
    {
        if (GUILayout.Button("Close"))
        {
            GetWindow<TransformRandomizerWindow>().Close();
        }
    }

    private void Button_Randomize()
    {
        GUI.enabled = Selection.transforms.Length > 0 && (_xAxisActive || _yAxisActive || _zAxisActive || (_currentTransformMode == TransformMode.Scale && _linkScaling));
        if (GUILayout.Button("Randomize!"))
        {
            if (ValidateCaptureData())
            {
                PerformRandomize();
            }
            else
            {
                if (EditorUtility.DisplayDialog("Randomizer",
                        "Selection differs from captured objects. Continuing will clear the captured objects!", "Continue", "Cancel"))
                {
                    _capturedData.Clear();
                    PerformRandomize();
                }
            }
        }
        GUI.enabled = true;
    }

    private void PerformRandomize()
    {
        Undo.RecordObjects(Selection.transforms, "Randomize");
        foreach (var transform in Selection.transforms)
        {
            switch (_currentTransformMode)
            {
                case TransformMode.Position:
                    var position = new Vector3
                    {
                        x = _xAxisActive
                            ? Random.Range(_positionXMin, _positionXMax)
                            : _currentTransformOperation == TransformOperation.Relative
                                ? 0
                                : _currentSpace == Space.Self
                                    ? transform.localPosition.x
                                    : transform.position.x,
                        y = _yAxisActive
                            ? Random.Range(_positionYMin, _positionYMax)
                            : _currentTransformOperation == TransformOperation.Relative
                                ? 0
                                : _currentSpace == Space.Self
                                    ? transform.localPosition.y
                                    : transform.position.y,
                        z = _zAxisActive
                            ? Random.Range(_positionZMin, _positionZMax)
                            : _currentTransformOperation == TransformOperation.Relative
                                ? 0
                                : _currentSpace == Space.Self
                                    ? transform.localPosition.z
                                    : transform.position.z
                    };

                    switch (_currentTransformOperation)
                    {
                        case TransformOperation.Relative:
                            transform.Translate(position, _currentSpace);
                            break;
                        case TransformOperation.Absolute:
                            switch (_currentSpace)
                            {
                                case Space.World:
                                    transform.position = position;
                                    break;
                                case Space.Self:
                                    transform.localPosition = position;
                                    break;
                                default:
                                    throw new System.ArgumentOutOfRangeException();
                            }
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException();
                    }

                    break;
                case TransformMode.Scale:
                    Vector3 scale;

                    if (_linkScaling)
                    {
                        scale = Vector3.one * Random.Range(_scaleXMin, _scaleXMax);
                    }
                    else
                    {

                        scale = new Vector3
                        {
                            x = _xAxisActive
                                ? Random.Range(_scaleXMin, _scaleXMax)
                                : _currentTransformOperation == TransformOperation.Relative
                                    ? 1
                                    : transform.localScale.x,
                            y = _yAxisActive
                                ? Random.Range(_scaleYMin, _scaleYMax)
                                : _currentTransformOperation == TransformOperation.Relative
                                    ? 1
                                    : transform.localScale.y,
                            z = _zAxisActive
                                ? Random.Range(_scaleZMin, _scaleZMax)
                                : _currentTransformOperation == TransformOperation.Relative
                                    ? 1
                                    : transform.localScale.z
                        };
                    }


                    switch (_currentTransformOperation)
                    {
                        case TransformOperation.Relative:
                            transform.localScale = Vector3.Scale(transform.localScale, scale);
                            break;
                        case TransformOperation.Absolute:
                            transform.localScale = scale;
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException();
                    }
                    break;
                case TransformMode.Rotation:

                    var rotation = new Vector3
                    {
                        x = _xAxisActive
                        ? Random.Range(_rotationXMin, _rotationXMax)
                        : _currentTransformOperation == TransformOperation.Relative
                            ? 0
                            : _currentSpace == Space.Self
                                ? transform.localRotation.x
                                : transform.rotation.x,
                        y = _yAxisActive
                        ? Random.Range(_rotationYMin, _rotationYMax)
                        : _currentTransformOperation == TransformOperation.Relative
                            ? 0
                            : _currentSpace == Space.Self
                                ? transform.localRotation.y
                                : transform.rotation.y,
                        z = _zAxisActive
                        ? Random.Range(_rotationZMin, _rotationZMax)
                        : _currentTransformOperation == TransformOperation.Relative
                            ? 0
                            : _currentSpace == Space.Self
                                ? transform.localRotation.z
                                : transform.rotation.z
                    };

                    switch (_currentTransformOperation)
                    {
                        case TransformOperation.Relative:
                            transform.Rotate(rotation, _currentSpace);
                            break;
                        case TransformOperation.Absolute:
                            switch (_currentSpace)
                            {
                                case Space.World:

                                    transform.rotation = Quaternion.Euler(rotation);
                                    break;
                                case Space.Self:
                                    transform.localRotation = Quaternion.Euler(rotation);
                                    break;
                                default:
                                    throw new System.ArgumentOutOfRangeException();
                            }
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }

    private void Button_CaptureState()
    {
        GUI.enabled = Selection.transforms.Length > 0;
        if (GUILayout.Button("Capture"))
        {
            _capturedData.Clear();
            foreach (var transform in Selection.transforms)
            {
                _capturedData.Add(transform, new TransformSaveData(transform));
            }
        }
        GUI.enabled = true;
    }

    private void Button_ClearState()
    {
        GUI.enabled = _capturedData.Count > 0;
        if (GUILayout.Button("Clear"))
        {
            _capturedData.Clear();
        }
        GUI.enabled = true;
    }
    private void Button_RestoreState()
    {
        GUI.enabled = _capturedData.Count > 0;
        if (GUILayout.Button("Restore"))
        {
            Undo.RecordObjects(_capturedData.Keys.ToArray(), "Restore Capture");
            foreach (var transform in _capturedData.Keys)
            {
                transform.position = _capturedData[transform].position;
                transform.localScale = _capturedData[transform].localScale;
                transform.rotation = _capturedData[transform].rotation;
            }
        }
        GUI.enabled = true;
    }

    private void Button_SelectState()
    {
        GUI.enabled = _capturedData.Count > 0;
        if (GUILayout.Button("Select"))
        {
            EditorTools.SelectList(_capturedData.Keys.ToList());
        }
        GUI.enabled = true;
    }

    private bool FieldIsModified(float min, float max, float defaultValue)
    {
        return !(min == max && max == defaultValue);
    }

    private Dictionary<Transform, TransformSaveData> _capturedData = new Dictionary<Transform, TransformSaveData>();
    private struct TransformSaveData
    {
        public Vector3 position;
        public Vector3 localScale;
        public Quaternion rotation;

        public TransformSaveData(Transform transform)
        {
            position = transform.position;
            localScale = transform.localScale;
            rotation = transform.rotation;
        }
    }

    private bool ValidateCaptureData()
    {
        if (_capturedData.Count == 0)
        {
            return true;
        }

        if (_capturedData.Count != Selection.transforms.Length)
        {
            return false;
        }

        foreach (var transform in Selection.transforms)
        {
            if (!_capturedData.ContainsKey(transform))
            {
                return false;
            }
        }
        return true;


    }

    private void Button_Help()
    {
        if (GUILayout.Button("Help"))
        {
            string helpText = @"How to use Randomizer: 
Hopefully most controls are self-explanatory. However, some 
features are not:

- Toggling Axis
    Click the axis labels (X/Y/Z) to toggle them on or off.
    Disabled axises will not be affected by the randomizer.

- Resetting values
    Click the 'reset' button on the right to reset the axis 
    field to its default values.

- Capturing state
    Capturing the state stores the original transform 
    settings for all selected objects. This lets you play 
    with the randomizer and then Restore the transforms to 
    their original settings.
    NOTE: Changing selection will clear the captured state;
    you will be warned when this happens.";
            EditorUtility.DisplayDialog("Randomizer Help", helpText, "Ok");
        }
    }
}
