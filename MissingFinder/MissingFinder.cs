using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissingFinder : EditorWindow
{
    [MenuItem("Tools/Missing Finder")]
    static void Init()
    {
        EditorWindow.GetWindow<MissingFinder>().Show();
    }

    enum FindTo
    {
        Scripts,
        Meshs
    }

    List<GameObject> foundObjects = new List<GameObject>();
    FindTo findTo;
    EditorMessageHandler editorMessageHandler;

    private void OnEnable()
    {
        editorMessageHandler = new EditorMessageHandler();
    }
    private void OnGUI()
    {
        findTo = (FindTo)EditorGUILayout.EnumPopup("Find To", findTo);
        if (GUILayout.Button("Find"))
        {
            Find();
            if (foundObjects.Count == 0)
            {
                editorMessageHandler.EmitMessage("Not Found!!", MessageType.Warning);
            }
        }
        ListiningsMissingObject();
        editorMessageHandler.MessageHandler();
    }

    void Find()
    {
        foundObjects.Clear();
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        switch (findTo)
        {
            case FindTo.Scripts:
                FindScripts(gameObjects);
                break;
            case FindTo.Meshs:
                FindMeshs(gameObjects);
                break;
        }
    }
    void FindScripts(GameObject[] gameObjects)
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Component[] components = (Component[])gameObjects[i].GetComponents<Component>();
            if (components != null && components.Length > 0)
            {
                for (int k = 0; k < components.Length; k++)
                {
                    if (components[k] == null)
                    {
                        foundObjects.Add(gameObjects[i]);
                        continue;
                    }
                }
            }
        }
    }
    void FindMeshs(GameObject[] gameObjects)
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            MeshFilter meshs = gameObjects[i].GetComponent<MeshFilter>();
            if (meshs != null)
            {
                if (meshs.sharedMesh == null)
                {
                    foundObjects.Add(gameObjects[i]);
                }
            }
            else
            {
                SkinnedMeshRenderer skinnedMeshRenderer = gameObjects[i].GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    if (skinnedMeshRenderer.sharedMesh == null)
                    {
                        foundObjects.Add(gameObjects[i]);
                    }
                }
            }
        }
    }

    void ListiningsMissingObject()
    {
        if (foundObjects.Count > 0)
        {
            EditorGUILayout.BeginVertical("Box");
            for (int i = 0; i < foundObjects.Count; i++)
            {
                if (foundObjects[i] != null)
                {
                    if (GUILayout.Button(foundObjects[i].name))
                    {
                        Selection.activeGameObject = foundObjects[i];
                        EditorGUIUtility.PingObject(foundObjects[i]);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
