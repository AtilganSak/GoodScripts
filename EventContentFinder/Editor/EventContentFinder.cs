#define TEXT_MESH
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Linq;
#if TEXT_MESH
using TMPro;
#endif

namespace EventFinder
{
    public class EventContentFinder : EditorWindow
    {
        [MenuItem("Window/Event Content Finder")]
        static void Init()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(EventContentFinder));
            window.title = "Event Content Finder";
            window.Show();
        }
        MonoBehaviour firstMono;
        GameObject firstObject;

        bool recursive;
        bool spelling;
        bool textMesh;

        string methodName;
        string gameObjectName;

        string[] searchStates;

        int selectedSearchStateIndex;

        const byte emitMessageLimit = 6;
        const string RECURSIVE = "recursive";
        const string SPELLING = "spelling";
        //const string TEXTMESH = "textmesh";
        const string TEXTMESHDEFINE = "#define TEXT_MESH";

        List<GameObject> foundObjects = new List<GameObject>();
        List<EditorMessage> editorMessages = new List<EditorMessage>();

        private void OnEnable()
        {
            methodName = "";
            gameObjectName = "";

            searchStates = new string[2];
            searchStates[0] = "Method Name";
            searchStates[1] = "Object Name";

            if (PlayerPrefs.HasKey(RECURSIVE))
                recursive = PlayerPrefs.GetInt(RECURSIVE) == 0 ? false : true;
            if (PlayerPrefs.HasKey(SPELLING))
                spelling = PlayerPrefs.GetInt(SPELLING) == 0 ? false : true;

            textMesh = IsDefineTEXTMESH();

            EditorSceneManager.activeSceneChangedInEditMode -= ChangedScene;
            EditorSceneManager.activeSceneChangedInEditMode += ChangedScene;
        }
        private void OnLostFocus()
        {
            PlayerPrefs.SetInt(RECURSIVE, recursive ? 1 : 0);
            PlayerPrefs.SetInt(SPELLING, spelling ? 1 : 0);
        }
        void ChangedScene(Scene scene, Scene scene1)
        {
            foundObjects.Clear();
        }
        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                if (GUI.enabled)
                    GUI.enabled = false;
                EditorGUILayout.HelpBox("Project is compailing, wait please!", MessageType.Warning);

            }
            else
            {
                if(!GUI.enabled)
                    GUI.enabled = true;

                selectedSearchStateIndex = EditorGUILayout.Popup("Search Target", selectedSearchStateIndex, searchStates);

                if (selectedSearchStateIndex == 0)
                {
                    methodName = EditorGUILayout.TextField("Method Name", methodName);
                }
                else if (selectedSearchStateIndex == 1)
                {
                    gameObjectName = EditorGUILayout.TextField("Object Name", gameObjectName);
                }

                recursive = EditorGUILayout.Toggle("Recursive", recursive);
                spelling = EditorGUILayout.Toggle("Spelling", spelling);
                EditorGUI.BeginChangeCheck();
                textMesh = EditorGUILayout.Toggle("Text Mesh", textMesh);
                if (EditorGUI.EndChangeCheck())
                {
                    if (textMesh)
                    {
                        DefineTEXTMESH();
                    }
                    else
                    {
                        RemoveDefineTEXTMESH();
                    }
                }

                if (GUILayout.Button("Clear"))
                {
                    foundObjects.Clear();
                    methodName = "";
                    gameObjectName = "";

                    GUI.FocusControl("");
                }
                if (GUILayout.Button("Play"))
                {
                    Play();
                }

                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    for (int i = 0; i < foundObjects.Count; i++)
                    {
                        if (GUILayout.Button(foundObjects[i].name))
                        {
                            Selection.activeObject = foundObjects[i];
                            EditorGUIUtility.PingObject(foundObjects[i]);
                        }
                    }
                }

                MessageHandler();
            }
        }
        void Play()
        {
            if (!Validate()) return;

            foundObjects.Clear();

            FindMonoBehaviours();
            FindUIComponents();

            if (foundObjects.Count == 0)
            {
                EmitMessage("Not found any objects.", MessageType.Info);
            }
            else
            {
                editorMessages.Clear();
            }
        }
        void FindUIComponents()
        {
            #region Look Button
            Button[] buttons = FindObjectsOfType<Button>();
            foreach (Button item in buttons)
            {
                firstObject = item.gameObject;
                FindObject(item.onClick);
            }
            #endregion
            #region Look Toggle
            Toggle[] toggles = FindObjectsOfType<Toggle>();
            foreach (Toggle item in toggles)
            {
                firstObject = item.gameObject;
                FindObject(item.onValueChanged);
            }
            #endregion
            #region Look Slider
            Slider[] sliders = FindObjectsOfType<Slider>();
            foreach (Slider item in sliders)
            {
                firstObject = item.gameObject;
                FindObject(item.onValueChanged);
            }
            #endregion
            #region Look Scrollbar
            Scrollbar[] scrollbars = FindObjectsOfType<Scrollbar>();
            foreach (Scrollbar item in scrollbars)
            {
                firstObject = item.gameObject;
                FindObject(item.onValueChanged);
            }
            #endregion
            #region Look Dropdown
            Dropdown[] dropdowns = FindObjectsOfType<Dropdown>();
            foreach (Dropdown item in dropdowns)
            {
                firstObject = item.gameObject;
                FindObject(item.onValueChanged);
            }
#if TEXT_MESH
            TMP_Dropdown[] TMP_dropdowns = FindObjectsOfType<TMP_Dropdown>();
            foreach (TMP_Dropdown item in TMP_dropdowns)
            {
                firstObject = item.gameObject;
                FindObject(item.onValueChanged);
            }
#endif
            #endregion
            #region Look Inputfield
            InputField[] inputFields = FindObjectsOfType<InputField>();
            foreach (InputField item in inputFields)
            {
                firstObject = item.gameObject;
                FindObject(item.onValueChanged);
                FindObject(item.onEndEdit);
            }
#if TEXT_MESH
            TMP_InputField[] TMP_inputFields = FindObjectsOfType<TMP_InputField>();
            foreach (TMP_InputField item in TMP_inputFields)
            {
                firstObject = item.gameObject;
                FindObject(item.onValueChanged);
                FindObject(item.onEndEdit);
            }
#endif
            #endregion
            #region Look ScrollRect
            ScrollRect[] scrollRects = FindObjectsOfType<ScrollRect>();
            foreach (ScrollRect item in scrollRects)
            {
                firstObject = item.gameObject;
                FindObject(item.onValueChanged);
            }
            #endregion
        }
        void FindMonoBehaviours()
        {
            MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
            foreach (var item in monoBehaviours)
            {
                firstMono = item;
                firstObject = firstMono.gameObject;
                FieldInfo[] fieldInfos = item.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                DetectFields(fieldInfos, item);
            }
        }
        void DetectFields(FieldInfo[] fieldInfos, object obj)
        {
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (obj != null)
                {
                    if (!fieldInfos[i].IsStatic)
                    {
                        if (!DetectUnityEvent(fieldInfos[i], obj))
                        {
                            Type tp = fieldInfos[i].FieldType;
                            if (tp.IsValueType || tp.IsClass)
                            {
                                if (typeof(IList).IsAssignableFrom(tp))
                                {
                                    IList genList = (IList)fieldInfos[i].GetValue(obj);
                                    if (genList != null)
                                    {
                                        for (int l = 0; l < genList.Count; l++)
                                        {
                                            object objs = genList[l];
                                            if (objs != null)
                                            {
                                                Type tp1 = genList[l].GetType();
                                                FieldInfo[] fieldInfos1 = tp1.GetFields();
                                                if (!recursive)
                                                {
                                                    for (int f = 0; f < fieldInfos1.Length; f++)
                                                    {
                                                        if (!fieldInfos1[f].IsStatic)
                                                        {
                                                            DetectUnityEvent(fieldInfos1[f], objs);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    DetectFields(fieldInfos1, objs);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    FieldInfo[] fieldInfos1 = tp.GetFields();
                                    object objs = fieldInfos[i].GetValue(obj);
                                    if (!recursive)
                                    {
                                        for (int f = 0; f < fieldInfos1.Length; f++)
                                        {
                                            if (objs != null)
                                            {
                                                if (!fieldInfos1[f].IsStatic)
                                                {
                                                    DetectUnityEvent(fieldInfos1[f], objs);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DetectFields(fieldInfos1, objs);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        bool DetectUnityEvent(FieldInfo fieldInfo, object obj)
        {
            if (fieldInfo.FieldType == typeof(UnityEvent))
            {
                UnityEvent tempUE = (UnityEvent)fieldInfo.GetValue(obj);
                FindObject(tempUE);

                return true;
            }
            else if (fieldInfo.FieldType == typeof(UnityEvent[]))
            {
                UnityEvent[] tempUE = (UnityEvent[])fieldInfo.GetValue(obj);
                if (tempUE != null)
                {
                    for (int l = 0; l < tempUE.Length; l++)
                    {
                        FindObject(tempUE[l]);
                    }

                    return true;
                }
            }
            else if (fieldInfo.FieldType == typeof(List<UnityEvent>))
            {
                List<UnityEvent> tempUE = (List<UnityEvent>)fieldInfo.GetValue(obj);
                if (tempUE != null)
                {
                    for (int l = 0; l < tempUE.Count; l++)
                    {
                        FindObject(tempUE[l]);
                    }

                    return true;
                }
            }

            return false;
        }
        void FindObject(UnityEventBase unityEvent)
        {
            if (unityEvent == null) return;

            for (int k = 0; k < unityEvent.GetPersistentEventCount(); k++)
            {
                if (selectedSearchStateIndex == 0)
                {
                    string eventMethodName = unityEvent.GetPersistentMethodName(k);
                    string useMethodName = methodName;
                    if (spelling)
                    {
                        eventMethodName = eventMethodName.ToLower().Trim();
                        useMethodName = useMethodName.ToLower().Trim();
                    }
                    if (eventMethodName == useMethodName)
                    {
                        foundObjects.Add(firstObject);
                    }
                }
                else if (selectedSearchStateIndex == 1)
                {
                    string eventTargetName = unityEvent.GetPersistentTarget(k).name;
                    string useObjectName = gameObjectName;
                    if (spelling)
                    {
                        eventTargetName = eventTargetName.ToLower().Trim();
                        useObjectName = useObjectName.ToLower().Trim();
                    }
                    if (eventTargetName == useObjectName)
                    {
                        foundObjects.Add(firstObject);
                    }
                }
            }
        }
        bool Validate()
        {
            if (selectedSearchStateIndex == 0)
            {
                if (methodName == "")
                {
                    EmitMessage("Please enter the Method Name!", MessageType.Warning);
                    return false;
                }
            }
            else if (selectedSearchStateIndex == 1)
            {
                if (gameObjectName == "")
                {
                    EmitMessage("Please enter the Object Name!", MessageType.Warning);
                    return false;
                }
            }
            return true;
        }

        #region Define Methods
        void DefineTEXTMESH()
        {
            string path = GetLocalScriptPath();
            List<string> lines = File.ReadAllLines(path).ToList();
            lines.Insert(0, TEXTMESHDEFINE);
            File.WriteAllLines(path, lines);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        void RemoveDefineTEXTMESH()
        {
            string path = GetLocalScriptPath();
            List<string> lines = File.ReadAllLines(path).ToList();
            lines.RemoveAt(0);
            File.WriteAllLines(path, lines);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        bool IsDefineTEXTMESH()
        {
            string path = GetLocalScriptPath();
            string content = File.ReadAllText(path);
            content = content.Substring(0, content.IndexOf("using")).Trim();
            return content.Contains(TEXTMESHDEFINE);
        }
        string GetLocalScriptPath()
        {
            MonoScript ms = MonoScript.FromScriptableObject(this);
            string m_ScriptFilePath = AssetDatabase.GetAssetPath(ms);

            return m_ScriptFilePath;
        }
        #endregion

        #region Message
        void EmitMessage(string _message, MessageType _messageType = MessageType.None, float _duringTime = 3)
        {
            if (editorMessages.Count < emitMessageLimit)
            {
                editorMessages.Add(
                    new EditorMessage
                    {
                        Message = _message,
                        MessageType = _messageType,
                        ShowDuring = _duringTime,
                        timer = (float)EditorApplication.timeSinceStartup
                    }
                );
            }
        }
        private void MessageHandler()
        {
            for (int i = 0; i < editorMessages.Count; i++)
            {
                EditorGUILayout.HelpBox(editorMessages[i].Message, editorMessages[i].MessageType);
                if (EditorApplication.timeSinceStartup - editorMessages[i].timer > editorMessages[i].ShowDuring)
                {
                    editorMessages.RemoveAt(i);
                }
            }
            Repaint();
        }
        #endregion
    }

    [System.Serializable]
    public struct EditorMessage
    {
        public string Message;
        public MessageType MessageType;
        public float ShowDuring;
        public float timer { get; set; }
    }
}
