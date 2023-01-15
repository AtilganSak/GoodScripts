using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

public static class PrefabHelper
{
    public static bool IsActivePrefabMode() => PrefabStageUtility.GetCurrentPrefabStage() == null ? false : true; 
    public static T AddComponentToPrefab<T>(GameObject prefabObject) where T : Component
    {
        PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(prefabObject);

        if(prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
        {
            string prefabPath = AssetDatabase.GetAssetPath(prefabObject);
            if(string.IsNullOrEmpty(prefabPath))
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
        if(string.IsNullOrEmpty(prefabPath))
            return;
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

        T component = prefabContents.GetComponent<T>();
        if(component)
        {
            GameObject.DestroyImmediate(component, true);
        }

        PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabContents);
    }
    public static GameObject LoadPrefabContents(GameObject prefabObject)
    {
        string prefabPath = AssetDatabase.GetAssetPath(prefabObject);
        if(string.IsNullOrEmpty(prefabPath))
            return null;
        return PrefabUtility.LoadPrefabContents(prefabPath);
    }
    public static void UnloadPrefabContents(GameObject prefabObject, GameObject prefabContents, bool saveChanges = true)
    {
        if(!prefabContents)
            return;
        if(saveChanges)
        {
            string prefabPath = AssetDatabase.GetAssetPath(prefabObject);
            if(string.IsNullOrEmpty(prefabPath))
                return;
            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
        }
        PrefabUtility.UnloadPrefabContents(prefabContents);
        if(prefabContents)
        {
            GameObject.DestroyImmediate(prefabContents);
        }
    }
    public static GameObject GetCorrespongingPrefabOfVariant(GameObject variant)
    {
        GameObject result = variant;
        PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(result);
        if(prefabType == PrefabAssetType.Variant)
        {
            if(PrefabUtility.IsPartOfNonAssetPrefabInstance(result))
                result = GetOutermostPrefabAssetRoot(result);

            prefabType = PrefabUtility.GetPrefabAssetType(result);
            if(prefabType == PrefabAssetType.Variant)
                result = GetOutermostPrefabAssetRoot(result);
        }
        return result;
    }
    public static GameObject GetOutermostPrefabAssetRoot(GameObject prefabInstance)
    {
        GameObject result = prefabInstance;
        GameObject newPrefabObject = PrefabUtility.GetCorrespondingObjectFromSource(result);
        if(newPrefabObject != null)
        {
            while(newPrefabObject.transform.parent != null)
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
        foreach(GameObject go in gameObjects)
        {
            prefabRoot = null;
            if(go != PrefabUtility.GetOutermostPrefabInstanceRoot(go))
                continue;
            prefabType = PrefabUtility.GetPrefabAssetType(go);
            if(prefabType == PrefabAssetType.Regular)
                prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(go);
            else if(prefabType == PrefabAssetType.Variant)
                prefabRoot = GetCorrespongingPrefabOfVariant(go);

            if(prefabRoot != null)
                result.Add(prefabRoot);
        }

        return result;
    }
}
