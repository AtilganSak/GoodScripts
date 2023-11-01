using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T[] assets = Resources.LoadAll<T>("GameAssets");
                if (assets != null && assets.Length > 0)
                    instance = assets[0];
            }
            return instance;
        }
    }
}