using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

public class DrawGizmos : MonoBehaviour
{
    public enum Shape { Box = 0, Sphere = 1, Capsule = 2 }

    [SerializeField] bool _draw = true;
    public bool Draw { get => _draw; set => _draw = value; }

    [SerializeField] Shape _shape;
    public Shape m_Shape { get => _shape; set => _shape = value; }

    [SerializeField] Color _color = Color.white;
    public Color Color { get => _color; set => _color = value; }

    [SerializeField] bool _wireFrame;
    public bool WireFrame { get => _wireFrame; set => _wireFrame = value; }

    [SerializeField] bool _freeSync;
    public bool FreeSycn { get => _freeSync; set => _freeSync = value; }

    //Box Parameters
    [ContextMenuItem("Reset", "ResetCubeSize")]
    [SerializeField] Vector3 _cubeSize = Vector3.one;
    public Vector3 CubeSize { get => _cubeSize; set => _cubeSize = value; }

    //Sphere Parameters
    [SerializeField] float _sphereRadius = 1;
    public float SphereRadius { get => _sphereRadius; set => _sphereRadius = value; }
    float c_SphereRadius = 1;

    //Capsule Parameters
    [SerializeField] float _capsuleRadius = 1;
    public float CapsuleRadius { get => _capsuleRadius; set => _capsuleRadius = value; }
    float c_CapsuleRadius = 1;
    [SerializeField] float _capsuleHeight = 3;
    public float CapsuleHeight { get => _capsuleHeight; set => _capsuleHeight = value; }
    float c_CapsuleHeight = 1;

    //Text Parameters
    [SerializeField] bool _drawText;
    public bool DrawText { get => _drawText; set => _drawText = value; }
    [SerializeField] string _label;
    public string Label { get => _label; set => _label = value; }
    [SerializeField] Color _labelColor = Color.white;
    [SerializeField] int _labelSize = 18;
    [SerializeField] FontStyle _labelFontSyle;
    [ContextMenuItem("Reset", "ResetLabelOffset")]
    [SerializeField] Vector3 _labelOffset;

    //Line Parameters
    [SerializeField] bool _drawLine;
    public bool DrawLine { get => _drawLine; set => _drawLine = value; }
    [SerializeField] float _thickness = 1;
    public float Thickness { get => _thickness; set => _thickness = value; }

    [SerializeField] Color _lineColor = Color.white;

    GUIStyle handleStyle = new GUIStyle();
    Vector3 labelPos;
    bool onceRun;

    [SerializeField] string selectedIconName;

    private void OnValidate()
    {
        if (FreeSycn && !onceRun)
        {
            CubeSize = transform.localScale;
            SphereRadius = c_SphereRadius;
            CapsuleRadius = c_CapsuleRadius;
            CapsuleHeight = c_CapsuleHeight;
            onceRun = true;
        }
        if (!FreeSycn && onceRun)
            onceRun = false;

        if (FreeSycn)
        {
            c_SphereRadius = SphereRadius;
            c_CapsuleRadius = CapsuleRadius;
            c_CapsuleHeight = CapsuleHeight;
        }
    }
    private void OnDrawGizmos()
    {
        if (!Draw) return;

        DrawShape();

        ShowText();

        ShowLine();

        DrawIcon();
    }
    void DrawShape()
    {
        if (!FreeSycn)
            Gizmos.matrix = transform.localToWorldMatrix;
        else
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        }

        Gizmos.color = Color;

        switch (m_Shape)
        {
            case Shape.Box:
                DrawCube();
                break;
            case Shape.Sphere:
                DrawSphere();
                break;
            case Shape.Capsule:
                DrawCap();
                break;
        }
    }
    void ShowText()
    {
#if UNITY_EDITOR
        if (_drawText)
        {
            handleStyle.fontStyle = _labelFontSyle;
            handleStyle.normal.textColor = _labelColor;
            handleStyle.fontSize = _labelSize;
            labelPos = transform.localPosition + _labelOffset;

            Handles.Label(labelPos, _label, handleStyle);
        }
#endif
    }
    void ShowLine()
    {
#if UNITY_EDITOR
        Vector3 startPos = transform.position;
        Vector3 endPos = labelPos;
        float halfHeight = (startPos.y - endPos.y) * 0.5f;
        Vector3 offset = Vector3.up * halfHeight;

        Handles.DrawBezier(
            startPos,
            endPos,
            startPos - offset,
            endPos + offset,
            _labelColor,
            EditorGUIUtility.whiteTexture,
            _thickness
            );
#endif
    }
    void DrawIcon()
    {
        if(selectedIconName != null && selectedIconName != "" && selectedIconName != "None")
            Gizmos.DrawIcon(transform.position, selectedIconName);
    }
    void DrawCube()
    {
        if (!FreeSycn)
            CubeSize = Vector3.one;

        if (!WireFrame)
            Gizmos.DrawCube(Vector3.zero, CubeSize);
        else
            Gizmos.DrawWireCube(Vector3.zero, CubeSize);
    }
    void DrawSphere()
    {
        if (!FreeSycn)
            SphereRadius = 1;

        if (!WireFrame)
            Gizmos.DrawSphere(Vector3.zero, SphereRadius);
        else
            Gizmos.DrawWireSphere(Vector3.zero, SphereRadius);
    }
    void DrawCap()
    {
        if (!FreeSycn)
        {
            CapsuleRadius = 0.5f;
            CapsuleHeight = 3;
        }

        if (!WireFrame)
            DrawCapsule(transform.position, transform.rotation, CapsuleRadius, CapsuleHeight, Color);
        else
            DrawCapsule(transform.position, transform.rotation, CapsuleRadius, CapsuleHeight, Color, true);
    }
    void ResetCubeSize()
    {
        _cubeSize = Vector3.one;
    }
    void ResetLabelOffset()
    {
        _labelOffset = Vector3.zero;
    }
#if UNITY_EDITOR
    void DrawCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color), bool _wireFrame = false)
    {
        if (_color != default(Color))
            Handles.color = _color;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            if (_wireFrame)
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            else
                Handles.DrawSolidArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            if (_wireFrame)
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            else
                Handles.DrawSolidArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            if (_wireFrame)
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            else
                Handles.DrawSolidArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            if (_wireFrame)
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            else
                Handles.DrawSolidArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            if (_wireFrame)
            {
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
            }
            else
            {
                Handles.DrawSolidDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawSolidDisc(Vector3.down * pointOffset, Vector3.up, _radius);
            }
        }
    }
#endif
}
#if UNITY_EDITOR
[CanEditMultipleObjects,CustomEditor(typeof(DrawGizmos))]
public class DrawGizmosEditor : Editor
{
    DrawGizmos script { get => target as DrawGizmos; }

    SerializedObject s_Script;

    SerializedProperty p_Shape;
    SerializedProperty p_Color;
    SerializedProperty p_WireFrame;
    SerializedProperty p_FreeSync;
    SerializedProperty p_CubeSize;
    SerializedProperty p_SphereRadius;
    SerializedProperty p_CapsuleRadius;
    SerializedProperty p_CapsuleHeight;
    SerializedProperty p_LabelColor;
    SerializedProperty p_LabelSize;
    SerializedProperty p_LabelFontStyle;
    SerializedProperty p_LabelOffset;
    SerializedProperty p_LineColor;
    SerializedProperty p_LineTickness;

    SerializedProperty p_SelectedIcon;

    int hashCode;

    string[] iconNames = new string[]
    {
        "None",
        "BuildSettings.Editor",
        "ageialogo",
        "Animation.Record@2x",
        "animationvisibilitytoggleon@2x",
        "AutoLightbakingOn@2x",
        "AvatarSelector@2x",
        "BuildSettings.Android",
        "BuildSettings.Facebook",
        "BuildSettings.iPhone",
        "BuildSettings.Metro",
        "BuildSettings.PSM",
        "BuildSettings.Xbox360"
    };

    int selectedIconIndex;

    private void OnEnable()
    {
        s_Script = new SerializedObject(script);
        p_Shape = s_Script.FindProperty("_shape");
        p_Color = s_Script.FindProperty("_color");
        p_WireFrame = s_Script.FindProperty("_wireFrame");
        p_FreeSync = s_Script.FindProperty("_freeSync");
        p_CubeSize = s_Script.FindProperty("_cubeSize");
        p_SphereRadius = s_Script.FindProperty("_sphereRadius");
        p_CapsuleRadius = s_Script.FindProperty("_capsuleRadius");
        p_CapsuleHeight = s_Script.FindProperty("_capsuleHeight");
        p_LabelColor = s_Script.FindProperty("_labelColor");
        p_LabelSize = s_Script.FindProperty("_labelSize");
        p_LabelFontStyle = s_Script.FindProperty("_labelFontSyle");
        p_LabelOffset = s_Script.FindProperty("_labelOffset");
        p_LineColor = s_Script.FindProperty("_lineColor");
        p_LineTickness = s_Script.FindProperty("_thickness");

        p_SelectedIcon = s_Script.FindProperty("selectedIconName");

        selectedIconIndex = 0;

        hashCode = script.gameObject.name.GetHashCode();

        if (PlayerPrefs.HasKey(hashCode.ToString()))
            selectedIconIndex = PlayerPrefs.GetInt(hashCode.ToString());
    }
    public override void OnInspectorGUI()
    {
        s_Script.Update();

        #region Draw Region
        using (new EditorGUILayout.VerticalScope("Box"))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                script.Draw = EditorGUILayout.Toggle(script.Draw, GUILayout.Width(20));
                EditorGUILayout.LabelField("Draw", EditorStyles.boldLabel);
                if (EditorGUI.EndChangeCheck())
                    SceneView.lastActiveSceneView.Repaint();
            }
            if (script.Draw)
            {
                EditorGUI.indentLevel++;
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(p_Shape);
                    EditorGUILayout.PropertyField(p_Color);
                    EditorGUILayout.PropertyField(p_WireFrame);
                    EditorGUILayout.PropertyField(p_FreeSync);
                    if (p_FreeSync.boolValue)
                    {
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            if ((DrawGizmos.Shape)p_Shape.enumValueIndex == DrawGizmos.Shape.Box)
                                EditorGUILayout.PropertyField(p_CubeSize);
                            else if ((DrawGizmos.Shape)p_Shape.enumValueIndex == DrawGizmos.Shape.Sphere)
                                EditorGUILayout.PropertyField(p_SphereRadius);
                            else if ((DrawGizmos.Shape)p_Shape.enumValueIndex == DrawGizmos.Shape.Capsule)
                            {
                                EditorGUILayout.PropertyField(p_CapsuleRadius);
                                EditorGUILayout.PropertyField(p_CapsuleHeight);
                            }
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    selectedIconIndex = EditorGUILayout.Popup("Icon", selectedIconIndex, iconNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        PlayerPrefs.SetInt(hashCode.ToString(), selectedIconIndex);
                    }

                    p_SelectedIcon.stringValue = iconNames[selectedIconIndex];
                }
                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Draw Text Region
        using (new EditorGUILayout.VerticalScope("Box"))
        {
            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.HorizontalScope())
            {
                script.DrawText = EditorGUILayout.Toggle(script.DrawText, GUILayout.Width(20));
                EditorGUILayout.LabelField("Draw Text", EditorStyles.boldLabel);
            }
            if (EditorGUI.EndChangeCheck())
                SceneView.lastActiveSceneView.Repaint();
            if (script.DrawText)
            {
                EditorGUI.indentLevel++;
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        script.Label = EditorGUILayout.TextArea(script.Label, GUILayout.Height(100));
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUIUtility.labelWidth = 100;
                            EditorGUILayout.PropertyField(p_LabelColor);
                            EditorGUILayout.PropertyField(p_LabelSize);
                            EditorGUIUtility.labelWidth = 209.75f;
                        }
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUIUtility.labelWidth = 130;
                            EditorGUILayout.PropertyField(p_LabelFontStyle);
                            EditorGUIUtility.labelWidth = 209.75f;
                        }
                        EditorGUILayout.PropertyField(p_LabelOffset);
                    }
                    EditorGUI.BeginChangeCheck();
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUI.indentLevel--;
                            script.DrawLine = EditorGUILayout.Toggle(script.DrawLine, GUILayout.Width(20));
                            EditorGUILayout.LabelField("Draw Line", EditorStyles.boldLabel);
                        }

                        if (script.DrawLine)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(p_LineColor);
                            EditorGUILayout.PropertyField(p_LineTickness);
                            EditorGUI.indentLevel--;
                        }
                    }
                    if (EditorGUI.EndChangeCheck())
                        SceneView.lastActiveSceneView.Repaint();
                }
            }
        }
        #endregion

        s_Script.ApplyModifiedProperties();
    }
}
#endif
