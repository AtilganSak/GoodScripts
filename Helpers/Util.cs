using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public static class Util
{
    public static bool IsOnline { get => Application.internetReachability == NetworkReachability.NotReachable; }
    public static string ConvertSecondsToFormattedTimeString(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"mm\:ss");
    }
    public static T RandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));
        int random = UnityEngine.Random.Range(0, values.Length);
        return (T)values.GetValue(random);
    }
    public static bool RandomBool
    {
        get { return UnityEngine.Random.value > 0.5F ? true : false; }
    }
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        for (int i = 0; i < results.Count; i++)
        {
            if (LayerMask.LayerToName(results[0].gameObject.layer) == "UI")
            {
                return true;
            }
        }
        return false;
    }
    public static float Remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }
    public static string FloatToMMss(float time)
    {
        return string.Format("{0:00}:{1:00}", Mathf.Floor(time / 60), Mathf.FloorToInt(time) % 60);
    }
    public static string FloatToMMssSS(float time)
    {
        return string.Format("{0:00}:{1:00}.{2:000}", Mathf.Floor(time / 60), Mathf.FloorToInt(time) % 60, Mathf.FloorToInt(time * 1000) % 1000);
    }
    public static int GetAgentTypeIDByName(string agentTypeName)
    {
        int count = NavMesh.GetSettingsCount();
        string[] agentTypeNames = new string[count + 2];
        for (var i = 0; i < count; i++)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            string name = NavMesh.GetSettingsNameFromID(id);
            if (name == agentTypeName)
            {
                return id;
            }
        }
        return -1;
    }
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1, float spatialBlend = 0)
    {
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = volume;
        audioSource.Play();
        UnityEngine.Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
    public static bool CompareLayerMasks(LayerMask mask1, LayerMask mask2)
    {
        return (mask1 & mask2) != 0;
    }
    public static IEnumerable<System.Type> GetDerivedTypesFor<TBase>()
    {
        var baseType = typeof(TBase);
        var assembly = Assembly.GetAssembly(baseType);
        return assembly.GetTypes().Where(baseType.IsAssignableFrom).Where(t => baseType != t);
    }
    public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs)
    {
        List<T> objects = new List<T>();
        foreach (System.Type type in
            Assembly.GetAssembly(typeof(T)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
        {
            objects.Add((T)Activator.CreateInstance(type, constructorArgs));
        }
        //objects.Sort();
        return objects;
    }
    public static float AngleDirection(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);
        if (dir > 0f)
            return 1f;
        else if (dir < 0f)
            return -1f;
        else
            return 0f;
    }
    public static Color HextToColor(string hex)
    {
        UnityEngine.ColorUtility.TryParseHtmlString("#" + hex, out Color returnColor);
        return returnColor;
    }
    public static Vector3 ScreenToWorld(Vector3 screenPosition, float zOffset = 15)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, zOffset));
    }
#if UNITY_EDITOR
    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;

    }
    #region Prefab
    public static bool IsActivePrefabMode() => PrefabStageUtility.GetCurrentPrefabStage() == null ? false : true;
    public static T AddComponentToPrefab<T>(GameObject prefabObject) where T : Component
    {
        PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(prefabObject);

        if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
        {
            string prefabPath = AssetDatabase.GetAssetPath(prefabObject);
            if (string.IsNullOrEmpty(prefabPath))
                return null;
            GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

            prefabContents.AddComponent<T>();

            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabContents);

            return prefabObject.GetComponent<T>();
        }

        return prefabObject.AddComponent<T>();
    }
    public static void RemoveComponentFromPrefab<T>(GameObject prefabObject) where T : Component
    {
        string prefabPath = AssetDatabase.GetAssetPath(prefabObject);
        if (string.IsNullOrEmpty(prefabPath))
            return;
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

        T component = prefabContents.GetComponent<T>();
        if (component)
        {
            GameObject.DestroyImmediate(component, true);
        }

        PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabContents);
    }
    public static GameObject LoadPrefabContents(GameObject prefabObject)
    {
        string prefabPath = AssetDatabase.GetAssetPath(prefabObject);
        if (string.IsNullOrEmpty(prefabPath))
            return null;
        return PrefabUtility.LoadPrefabContents(prefabPath);
    }
    public static void UnloadPrefabContents(GameObject prefabObject, GameObject prefabContents, bool saveChanges = true)
    {
        if (!prefabContents)
            return;
        if (saveChanges)
        {
            string prefabPath = AssetDatabase.GetAssetPath(prefabObject);
            if (string.IsNullOrEmpty(prefabPath))
                return;
            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
        }
        PrefabUtility.UnloadPrefabContents(prefabContents);
        if (prefabContents)
        {
            GameObject.DestroyImmediate(prefabContents);
        }
    }
    public static GameObject GetCorrespongingPrefabOfVariant(GameObject variant)
    {
        GameObject result = variant;
        PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(result);
        if (prefabType == PrefabAssetType.Variant)
        {
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(result))
                result = GetOutermostPrefabAssetRoot(result);

            prefabType = PrefabUtility.GetPrefabAssetType(result);
            if (prefabType == PrefabAssetType.Variant)
                result = GetOutermostPrefabAssetRoot(result);
        }
        return result;
    }
    public static GameObject GetOutermostPrefabAssetRoot(GameObject prefabInstance)
    {
        GameObject result = prefabInstance;
        GameObject newPrefabObject = PrefabUtility.GetCorrespondingObjectFromSource(result);
        if (newPrefabObject != null)
        {
            while (newPrefabObject.transform.parent != null)
                newPrefabObject = newPrefabObject.transform.parent.gameObject;
            result = newPrefabObject;
        }
        return result;
    }
    public static List<GameObject> GetCorrespondingPrefabAssetsOfGameObjects(GameObject[] gameObjects)
    {
        List<GameObject> result = new List<GameObject>();
        PrefabAssetType prefabType;
        GameObject prefabRoot;
        foreach (GameObject go in gameObjects)
        {
            prefabRoot = null;
            if (go != PrefabUtility.GetOutermostPrefabInstanceRoot(go))
                continue;
            prefabType = PrefabUtility.GetPrefabAssetType(go);
            if (prefabType == PrefabAssetType.Regular)
                prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(go);
            else if (prefabType == PrefabAssetType.Variant)
                prefabRoot = GetCorrespongingPrefabOfVariant(go);

            if (prefabRoot != null)
                result.Add(prefabRoot);
        }

        return result;
    }
    #endregion

    public static GameObject[] GetOrderedSelectedObjects()
    {
        return Selection.gameObjects.OrderBy(go => go.transform.GetSiblingIndex()).ToArray();
    }

    [MenuItem("Editor Extensions/Prefab Shortcuts/Apply Prefab Changes #q")]
    static void ApplysPrefabChanges()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.Log("Nothing selected");
            return;
        }

        foreach (GameObject obj in selectedObjects)
        {
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);

            if (prefabRoot == null)
            {
                Debug.Log("Selected object is not part of a prefab: " + obj.name);
                continue;
            }

            UnityEngine.Object prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(prefabRoot);

            if (prefabSource == null)
            {
                Debug.Log("Selected object is not a prefab instance: " + prefabRoot.name);
                continue;
            }

            string path = AssetDatabase.GetAssetPath(prefabSource);

            if (PrefabUtility.SaveAsPrefabAssetAndConnect(prefabRoot, path, InteractionMode.UserAction))
            {
                Debug.Log("Prefab updated: " + path);
            }
            else
            {
                Debug.Log("Could not update prefab: " + path);
            }
        }
    }

    [MenuItem("Editor Extensions/Prefab Shortcuts/Revert Prefab Changes #w")]
    static public void RevertPrefabChanges()
    {
        GameObject[] objs = Selection.gameObjects;
        if (objs != null)
        {
            foreach (var obj in objs)
            {
                PrefabUtility.RevertPrefabInstance(obj, InteractionMode.UserAction);
            }
        }
        else
        {
            Debug.Log("Nothing selected");
        }
    }
    [MenuItem("Editor Extensions/Prefab Shortcuts/Select Prefab Asset %q")]
    static public void SelectPrefabAsset()
    {
        if (Selection.activeGameObject == null) return;

        GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject) as GameObject;

        if (prefabSource != null)
        {
            EditorGUIUtility.PingObject(prefabSource);
        }
        else
        {
            Debug.Log("Selected object is not a prefab instance");
        }
    }
    [MenuItem("GameObject/Create Empty Same Name", false, 0)]
    static void CreateEmptySameName()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            GameObject newGameObject = new GameObject(selectedObject.name);
            Undo.RegisterCreatedObjectUndo(newGameObject, "cratedobejctsamename");
        }
    }

    //[MenuItem("Editor/Auto Refresh")]
    //private static void AutoRefreshToggle()
    //{
    //    var status = EditorPrefs.GetBool("kAutoRefresh");

    //    EditorPrefs.SetBool("kAutoRefresh", !status);
    //}

    //[MenuItem("Editor/Auto Refresh", true)]
    //private static bool AutoRefreshToggleValidation()
    //{
    //    var status = EditorPrefs.GetBool("kAutoRefresh");

    //    Menu.SetChecked("Editor/Auto Refresh", status);

    //    return true;
    //}

    //[MenuItem("Editor/Refresh %r")]
    //private static void Refresh()
    //{
    //    Debug.Log("Request script reload.");

    //    EditorApplication.UnlockReloadAssemblies();

    //    AssetDatabase.Refresh();

    //    EditorUtility.RequestScriptReload();
    //}
#endif
}