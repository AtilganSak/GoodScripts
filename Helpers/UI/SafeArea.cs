using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform Panel;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        Panel = GetComponent<RectTransform>();
        Refresh();
    }

    //void Update()
    //{
    //    Refresh();
    //}

    void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (safeArea != LastSafeArea)
            ApplySafeArea(safeArea);
    }

    Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;

        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;
    }

    //RectTransform rectTransform;
    //Rect safeArea;
    //Vector2 minArchor;
    //Vector2 maxArchor;

    //private void Awake()
    //{
    //    rectTransform = GetComponent<RectTransform>();
    //    safeArea = Screen.safeArea;
    //    minArchor = safeArea.position;
    //    maxArchor = minArchor + safeArea.position;

    //    minArchor.x /= Screen.width;
    //    minArchor.y /= Screen.height;
    //    maxArchor.x /= Screen.width;
    //    maxArchor.y /= Screen.height;

    //    rectTransform.anchorMin = minArchor;
    //    rectTransform.anchorMax = maxArchor;
    //}
}
