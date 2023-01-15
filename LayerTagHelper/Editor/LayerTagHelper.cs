using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LayerTagHelper
{
    public static void AddTag(string tag)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for(int i = 0; i < tags.arraySize; ++i)
            {
                if(tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    return;     // Tag already present, nothing to do.
                }
            }

            tags.InsertArrayElementAtIndex(0);
            tags.GetArrayElementAtIndex(0).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
    public static void RemoveTag(string tag)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for(int i = 0; i < tags.arraySize; ++i)
            {
                if(tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    tags.DeleteArrayElementAtIndex(i);
                    break;
                }
            }
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
    public static void EditTag(string presentTag, string newTag)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for(int i = 0; i < tags.arraySize; ++i)
            {
                if(tags.GetArrayElementAtIndex(i).stringValue == presentTag)
                {
                    tags.GetArrayElementAtIndex(i).stringValue = newTag;
                    break;
                }
            }
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
    public static bool IsPresentTag(string tag)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for(int i = 0; i < tags.arraySize; ++i)
            {
                if(tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static void AddLayer(string layer)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty layers = so.FindProperty("layers");

            int emptyIndex = -1;
            for(int i = 8; i < layers.arraySize; i++)
            {
                if(layers.GetArrayElementAtIndex(i).stringValue == layer)
                {
                    return;
                }
                if(layers.GetArrayElementAtIndex(i).stringValue == "")
                {
                    if(emptyIndex == -1)
                        emptyIndex = i;
                }
            }
            if(emptyIndex != -1)
            {
                layers.GetArrayElementAtIndex(emptyIndex).stringValue = layer;
            }
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
    public static void RemoveLayer(string layer)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty layers = so.FindProperty("layers");

            for(int i = 8; i < layers.arraySize; i++)
            {
                if(layers.GetArrayElementAtIndex(i).stringValue == layer)
                {
                    layers.GetArrayElementAtIndex(i).stringValue = "";
                    break;
                }
            }
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
    public static void EditLayer(string presentLayer, string newLayer)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty layers = so.FindProperty("layers");

            for(int i = 8; i < layers.arraySize; i++)
            {
                if(layers.GetArrayElementAtIndex(i).stringValue == presentLayer)
                {
                    layers.GetArrayElementAtIndex(i).stringValue = newLayer;
                    break;
                }
            }
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
    public static bool IsPresentLayer(string layer)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty layers = so.FindProperty("layers");

            for(int i = 8; i < layers.arraySize; i++)
            {
                if(layers.GetArrayElementAtIndex(i).stringValue == layer)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
