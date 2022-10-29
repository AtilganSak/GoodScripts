using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
#if TextMeshPro
using TMPro;
#endif

public static class Extensions
{
    private static System.Random mRandom = new System.Random();

    #region String
    public static string ConvertSecondsToFormattedTimeString(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"mm\:ss");
    }
    public static string Triming(string text)
    {
        string newText = string.Join(" ", text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
        return newText;
    }
    public static string Triming(string text, string sperator, string targetChar)
    {
        string newText = string.Join(sperator.ToString(), text.Split(new string[] { targetChar.ToString() }, StringSplitOptions.RemoveEmptyEntries));
        return newText;
    }
    #endregion
    #region Color
    public static void SetAlpha(this Color color, float alpha)
    {
        if(alpha > 1)
            alpha = 1;
        if(alpha < 0)
            alpha = 0;
        color = new Color(color.r, color.g, color.b, alpha);
    }
    #endregion
    #region Button
    public static void ChangeName(this Button button, string name)
    {
        if(button.transform.childCount > 0)
        {
#if TextMeshPro
            if(button.transform.GetChild(0).GetComponent<TMP_Text>())
            {
                button.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
            }
#else
            if(button.transform.GetChild(0).GetComponent<Text>())
            {
                button.transform.GetChild(0).GetComponent<Text>().text = name;
            }
#endif
        }
    }
    public static void ChangeImage(this Button button, Sprite sprite)
    {
        if(button.GetComponent<Image>())
        {
            if(sprite != null)
                button.GetComponent<Image>().sprite = sprite;
        }
    }
    public static void ChangeTextColor(this Button button, Color color)
    {
        if(button.transform.childCount > 0)
        {
#if TextMeshPro
            if(button.transform.GetChild(0).GetComponent<TMP_Text>())
            {
                button.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
            }
#else
            if(button.transform.GetChild(0).GetComponent<Text>())
            {
                button.transform.GetChild(0).GetComponent<Text>().color = color;
            }
#endif
        }
    }
    #endregion
    #region List
    public static Dictionary<TKey, TValue> ReverseDict<TKey, TValue>(this Dictionary<TKey, TValue> _source)
    {
        Dictionary<TKey, TValue> reversedDict = new Dictionary<TKey, TValue>();
        int sourceItemCount = _source.Count;
        for(int i = sourceItemCount - 1; i >= 0; i--)
        {
            KeyValuePair<TKey, TValue> kvp = _source.ElementAt(i);
            reversedDict.Add(kvp.Key, kvp.Value);
        }
        return reversedDict;
    }
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while(n > 1)
        {
            n--;
            int k = mRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static T GetRandom<T>(this IList<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    public static T GetLast<T>(this IList<T> list)
    {
        return list[list.Count - 1];
    }
    public static T GetFirst<T>(this IList<T> list)
    {
        return list[0];
    }
    #endregion
    #region Vector
    public static Vector3 AddValueX(this Vector3 vector, float value)
    {
        return new Vector3(vector.x + value, vector.y, vector.z);
    }
    public static Vector3 AddValueY(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, vector.y + value, vector.z);
    }
    public static Vector3 AddValueZ(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, vector.y, vector.z + value);
    }
    public static void ABS(this Vector3 v)
    {
        v.x = Math.Abs(v.x);
        v.y = Math.Abs(v.y);
        v.z = Math.Abs(v.z);
    }
    public static void InverseABS(this Vector3 v)
    {
        v.x = -Math.Abs(v.x);
        v.y = -Math.Abs(v.y);
        v.z = -Math.Abs(v.z);
    }
    #endregion
    #region Editor
#if UNITY_EDITOR
    [MenuItem("EditorExtensions/Create Empty Same Name", false, 0)]
    static void CreateEmptySameName()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if(selectedObject != null)
        {
            GameObject newGameObject = new GameObject(selectedObject.name);
            Undo.RegisterCreatedObjectUndo(newGameObject, "cratedobejctsamename");
        }
    }
    [MenuItem("EditorExtensions/Dublicate Child &%d")]
    static void CreateDublicateChild()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        if(gameObjects != null)
        {
            if(gameObjects.Length > 0)
            {
                GameObject[] gos = new GameObject[gameObjects.Length];
                for(int i = 0; i < gameObjects.Length; i++)
                {
                    gos[i] = MonoBehaviour.Instantiate(gameObjects[i]);
                    gos[i].transform.SetParent(gameObjects[i].transform);
                    Undo.RegisterCreatedObjectUndo(gos[i], "createdublicatechild");
                }
                Selection.objects = gos;
            }
        }
    }
#endif
    #endregion
}
