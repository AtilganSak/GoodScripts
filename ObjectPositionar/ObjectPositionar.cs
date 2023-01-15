using UnityEditor;
using UnityEngine;

namespace Highborn
{
    public class ObjectPositionar : EditorWindow
    {
        GameObject referencePoint;
        GameObject generateObject;

        int count;
        float forwardOffset;
        float cosEffect = 5;
        float cosOffset = 5;
        float sinEffect = 5;
        float sinOffset = 5;

        [MenuItem("Tools/Object Positionar")]
        static void Init()
        {
            ObjectPositionar window = (ObjectPositionar)EditorWindow.GetWindow(typeof(ObjectPositionar));
            window.Show();
        }

        void OnGUI()
        {
            referencePoint = Selection.activeGameObject;
            if(referencePoint != null && referencePoint.scene != null)
            {
                EditorGUI.BeginChangeCheck();
                generateObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Generate Object"), generateObject, typeof(GameObject));
                if(generateObject != null)
                {
                    count = EditorGUILayout.IntField("Count", count);
                    forwardOffset = EditorGUILayout.FloatField("Forward Offset", forwardOffset);
                    cosEffect = EditorGUILayout.FloatField("Cos Effect", cosEffect);
                    cosOffset = EditorGUILayout.FloatField("Cos Offset", cosOffset);
                    sinEffect = EditorGUILayout.FloatField("Sin Effect", sinEffect);
                    sinOffset = EditorGUILayout.FloatField("Sin Offset", sinOffset);
                }
                else
                {
                    EditorGUILayout.LabelField("Please attach generate object");
                }
                if(EditorGUI.EndChangeCheck())
                {
                    Generate();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Please select reference point");
            }
        }
        private void Generate()
        {
            if(generateObject == null) return;

            ClearnChilds();
            
            int undoID = Undo.GetCurrentGroup();

            Vector3 pos = Vector3.zero;
            for(int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                GameObject newGo = null;
                if(PrefabUtility.IsPartOfAnyPrefab(generateObject))
                    newGo = (GameObject)PrefabUtility.InstantiatePrefab(generateObject, referencePoint.transform);
                else
                    newGo = Instantiate(generateObject, referencePoint.transform);
                pos += referencePoint.transform.forward * forwardOffset;
                pos.x = Mathf.Cos(t * cosEffect) * cosOffset;
                pos.y = Mathf.Sin(t * sinEffect) * sinOffset;
                newGo.transform.localPosition = pos;
                Undo.RegisterCreatedObjectUndo(newGo, "New GO");
                Undo.CollapseUndoOperations(undoID);
            }
            EditorUtility.SetDirty(this);
        }
        void ClearnChilds()
        {
            int childCount = referencePoint.transform.childCount;
            for(int i = 0; i < childCount; i++)
            {
                Undo.DestroyObjectImmediate(referencePoint.transform.GetChild(0).gameObject);
            }
        }
    }
}