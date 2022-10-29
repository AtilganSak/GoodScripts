using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Timers;
using System.Linq;

#if UNITY_EDITOR

public class ObjectCounter : EditorWindow
{    
    GameObject[] objects;

    int totalObjectCount;
    int activeObjectCount;
    int deactiveObjectCount;
    int staticObjectCount;

    [MenuItem("Tools/ObjectCounter")]
    static void Init() 
    {
        GetWindow(typeof(ObjectCounter), true, "Counter", true);
    }     
    
    private void OnGUI()
    {                        
        totalObjectCount = Selection.gameObjects.Length;

        objects = new GameObject[totalObjectCount];
        objects = Selection.gameObjects;

        activeObjectCount = objects.Where(x => x.activeInHierarchy).ToArray().Length;
        deactiveObjectCount = objects.Where(x => x.activeInHierarchy == false).ToArray().Length;
        staticObjectCount = objects.Where(x => x.isStatic).ToArray().Length;

        GUILayout.Box("Active Objects: " + activeObjectCount);
        GUILayout.Box("Deactive Objects: " + deactiveObjectCount);
        GUILayout.Box("Static Objects: " + staticObjectCount);

        Repaint();                
    }  
}
#endif