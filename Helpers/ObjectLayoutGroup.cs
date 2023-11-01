using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectLayoutGroup : MonoBehaviour
{
    public float width = 1;
    public float horizontalSpacing = 2;

    public void Clear()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    [Button]
    public void AlignHorizontal()
    {
        float childCount = transform.childCount;

        float totalWidth = width * childCount;
        float totalSpacing = horizontalSpacing * (childCount - 1);
        float startX = (totalWidth + totalSpacing) / 2f;        

        Vector3 newPosition = new Vector3(-startX, 0, 0);
        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).localPosition = newPosition;
            newPosition.x += (width + horizontalSpacing + (width / 2));
        }
    }
}
