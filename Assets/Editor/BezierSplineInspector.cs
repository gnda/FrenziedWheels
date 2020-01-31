using Spline;
using UnityEditor;
using UnityEngine;

/*
 * Code heavily inspired by
 * https://catlikecoding.com/unity/tutorials/curves-and-splines/
 */
namespace Editor
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : UnityEditor.Editor
    {
        public BezierSpline spline;
        public Transform handleTransform;
        public Quaternion handleRotation;
        private const float handleSize = 0.04f;
        private const float pickSize = 0.06f;
	
        private int selectedIndex = -1;
        
        private const int stepsPerCurve = 10;
        private const float directionScale = 0.5f;
        
        private bool addMode, drawBezierSpline;

        private static Color[] modeColors =
        {
            Color.white,
            Color.yellow,
            Color.cyan
        };

        private void OnSceneGUI()
        {
            Event e = Event.current;
            spline = target as BezierSpline;
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? 
                handleTransform.rotation : Quaternion.identity;

            if (addMode) {
                if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
                    RaycastHit h;
                    
                    if (!Physics.Raycast(worldRay, out h))
                    {
                        EditorGUI.BeginChangeCheck();
                        spline.AddPoint(worldRay.GetPoint(0f));
                        if (EditorGUI.EndChangeCheck()) {
                            Undo.RecordObject(spline, "Add Points Mode");
                            EditorUtility.SetDirty(spline);
                        }
                    }

                    e.Use();
                }
            }
            
            if (spline.ControlPointCount <= 0) return;
            
            for (int i = 1; i < spline.ControlPointCount; i++)
            {
                Handles.color = Color.gray;
                if (!drawBezierSpline) {
                    Handles.DrawLine(ShowPoint(i - 1), ShowPoint(i));
                }
            }
            
            if (drawBezierSpline) {
                DrawBezier();
            }
            SceneView.RepaintAll();
        }

        public override void OnInspectorGUI()
        {
            spline = target as BezierSpline;
            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Toggle Loop");
                EditorUtility.SetDirty(spline);
                spline.Loop = loop;
            }
            addMode = GUILayout.Toggle(addMode, "Add Points Mode", "Button");
            ActiveEditorTracker.sharedTracker.isLocked = addMode;
            if (addMode)
            {
                EditorGUI.BeginChangeCheck();
                bool lockXAxis = EditorGUILayout.Toggle("Lock X Axis", spline.LockXAxis);
                bool lockYAxis = EditorGUILayout.Toggle("Lock Y Axis", spline.LockYAxis);
                bool lockZAxis = EditorGUILayout.Toggle("Lock Z Axis", spline.LockZAxis);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Toggle Lock X Axis");
                    Undo.RecordObject(spline, "Toggle Lock Y Axis");
                    Undo.RecordObject(spline, "Toggle Lock Z Axis");
                    EditorUtility.SetDirty(spline);
                    spline.LockXAxis = lockXAxis;
                    spline.LockYAxis = lockYAxis;
                    spline.LockZAxis = lockZAxis;
                }
            }
            drawBezierSpline = GUILayout.Toggle(drawBezierSpline, 
                "Draw Bezier Spline", "Button");
            if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
                DrawSelectedPointInspector();
            if (addMode) {
                Undo.RecordObject(spline, "Add Points Mode");
            }
        }

        private void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point");
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", 
                spline.GetControlPoint(selectedIndex));
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(selectedIndex, point);
            }
            EditorGUI.BeginChangeCheck();
            BezierControlPointMode mode = (BezierControlPointMode)
                EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Change Point Mode");
                spline.SetControlPointMode(selectedIndex, mode);
                EditorUtility.SetDirty(spline);
            }
        }

        private void DrawBezier()
        {
            Handles.color = Color.green;
            Vector3 point = spline.GetPoint(0f);
            Handles.DrawLine(point, point + spline.GetDirection(0f) * 
                directionScale);
            int steps = stepsPerCurve * spline.CurveCount;
            for (int i = 1; i <= steps; i++)
            {
                Vector3 lineEnd = spline.GetPoint(i / (float)steps);
                Handles.color = Color.white;
                Handles.DrawLine(point, lineEnd);
                Handles.color = Color.green;
                Handles.DrawLine(lineEnd, lineEnd + 
                    spline.GetDirection(i / (float)steps) * directionScale);
                point = lineEnd;
            }
        }

        private Vector3 ShowPoint(int index)
        {
            Vector3 point = spline.GetControlPoint(index);

            float size = HandleUtility.GetHandleSize(point);
            if (index == 0) {
                size *= 2f;
            }
            Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
            if (Handles.Button(point, handleRotation, size * handleSize,
                size * pickSize, Handles.DotHandleCap)) {
                selectedIndex = index;
                Repaint();
            }
            if (selectedIndex == index) {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (Event.current != null && 
                    Event.current.isKey && 
                    Event.current.type.Equals(EventType.KeyDown) && 
                    Event.current.keyCode == KeyCode.Delete)
                {
                    spline.RemovePoint(point);
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                }
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlPoint(index, 
                        handleTransform.InverseTransformPoint(point));
                }
            }
            
            return point;
        }
    }
}
