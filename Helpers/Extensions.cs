using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public struct MinMaxPos
{
    public Vector3 minHeightPosition;
    public Vector3 maxHeightPosition;

    public Vector3 minWidthPosition;
    public Vector3 maxWidthPosition;
}
public static class Extensions
{
    #region Bounds Extensions
    public static Bounds GrowBounds(this Bounds a, Bounds b)
    {
        Vector3 max = Vector3.Max(a.max, b.max);
        Vector3 min = Vector3.Min(a.min, b.min);

        a = new Bounds((max + min) * 0.5f, max - min);
        return a;
    }
    #endregion

    #region Color Extensions
    public static Color SetAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
    #endregion

    #region Graphic Extensions
    public static void SetPassive(this Graphic graphic)
    {
        graphic.color = new Color(graphic.color.r / 2, graphic.color.g / 2, graphic.color.b / 2, graphic.color.a);        
    }
    public static void SetActive(this Graphic graphic)
    {
        graphic.color = new Color(graphic.color.r * 2, graphic.color.g * 2, graphic.color.b * 2, graphic.color.a);        
    }
    public static void SetPassive(this SpriteRenderer graphic)
    {
        graphic.color = new Color(graphic.color.r / 2, graphic.color.g / 2, graphic.color.b / 2, graphic.color.a);        
    }
    public static void SetActive(this SpriteRenderer graphic)
    {
        graphic.color = new Color(graphic.color.r * 2, graphic.color.g * 2, graphic.color.b * 2, graphic.color.a);        
    }
    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
    }
    #endregion
    
    #region MonoBehaviour Extensions
    private static string GetMethodName(Expression<Action> expr)
    {
        return ((MethodCallExpression)expr.Body).Method.Name;
    }
    public static void Invoke(this MonoBehaviour monoBehaviour, Expression<Action> expr, float time)
    {
        if (((MethodCallExpression)expr.Body).Arguments.Count > 0)
            throw new ArgumentException($"Can't invoke method '{GetMethodName(expr)}' with parameters.'");

        monoBehaviour.Invoke(GetMethodName(expr), time);
    }
    public static bool IsInvoking(this MonoBehaviour monoBehaviour, Expression<Action> expr)
    {
        return monoBehaviour.IsInvoking(GetMethodName(expr));
    }
    public static void CancelInvoke(this MonoBehaviour monoBehaviour, Expression<Action> expr)
    {
        monoBehaviour.CancelInvoke(GetMethodName(expr));
    }
    public static void InvokeRepeating(this MonoBehaviour monoBehaviour, Expression<Action> expr, float time, float repeatRate)
    {
        monoBehaviour.InvokeRepeating(GetMethodName(expr), time, repeatRate);
    }
    #endregion
    
    #region IList Extensions
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random rnd = new System.Random();
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static T GetRandom<T>(this IList<T> list)
    {
        if (list.Count == 1)
            return list[0];
        else
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
    public static int ElementCount(this IList list)
    {
        return list.Count - 1;
    }
    #endregion

    #region Dictionary Extensions
    public static Dictionary<TKey, TValue> ReverseDict<TKey, TValue>(this Dictionary<TKey, TValue> _source)
    {
        Dictionary<TKey, TValue> reversedDict = new Dictionary<TKey, TValue>();
        int sourceItemCount = _source.Count;
        for (int i = sourceItemCount - 1; i >= 0; i--)
        {
            KeyValuePair<TKey, TValue> kvp = _source.ElementAt(i);
            reversedDict.Add(kvp.Key, kvp.Value);
        }
        return reversedDict;
    }
    #endregion

    #region Vector Extensions
    public static Quaternion GetQuaternion(this Vector3 vec3)
    {
        return Quaternion.Euler(vec3);
    }
    public static Vector2 MakePixelPerfect(this Vector2 position)
    {
        return new Vector2((int)position.x, (int)position.y);
    }
    public static Vector2 Rotate(this Vector2 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
    }
    public static float Angle(this Vector2 direction)
    {
        return direction.y > 0
                   ? Vector2.Angle(new Vector2(1, 0), direction)
                   : -Vector2.Angle(new Vector2(1, 0), direction);
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

    #region Camera Extensions
    public static MinMaxPos GetBounds(this Camera camera)
    {
        MinMaxPos result = new MinMaxPos();
        float verticalHeightSeen = Camera.main.orthographicSize * 2.0f;
        float verticalWidthSeen = verticalHeightSeen * Camera.main.aspect;

        result.maxHeightPosition = camera.transform.position + (new Vector3(0, verticalHeightSeen / 2, 0));
        result.minHeightPosition = camera.transform.position - (new Vector3(0, verticalHeightSeen / 2, 0));

        result.maxWidthPosition = camera.transform.position + (new Vector3(verticalWidthSeen / 2, 0, 0));
        result.minWidthPosition = camera.transform.position - (new Vector3(verticalWidthSeen / 2, 0, 0));

        return result;
    }
    #endregion
    
    #region Sprite Renderer Extensions
    public static Vector3 GetTopRight(this SpriteRenderer spriteRenderer)
    {
        return spriteRenderer.bounds.max;
    }
    public static Vector3 GetTopLeft(this SpriteRenderer spriteRenderer)
    {
        return new Vector3(spriteRenderer.bounds.min.x, spriteRenderer.bounds.max.y);
    }
    public static Vector3 GetBotLeft(this SpriteRenderer spriteRenderer)
    {
        return spriteRenderer.bounds.min;
    }    
    public static Vector3 GetBotRight(this SpriteRenderer spriteRenderer)
    {
        return new Vector3(spriteRenderer.bounds.max.x, spriteRenderer.bounds.min.y);
    }
    public static Vector3 GetCroppedTopRight(this SpriteRenderer spriteRenderer)
    {
        Rect croppedRect = new Rect(
            (spriteRenderer.sprite.textureRectOffset.x - spriteRenderer.sprite.rect.width / 2f) / spriteRenderer.sprite.pixelsPerUnit,
            (spriteRenderer.sprite.textureRectOffset.y - spriteRenderer.sprite.rect.height / 2f) / spriteRenderer.sprite.pixelsPerUnit,
            spriteRenderer.sprite.textureRect.width / spriteRenderer.sprite.pixelsPerUnit,
            spriteRenderer.sprite.textureRect.height / spriteRenderer.sprite.pixelsPerUnit);

        Vector3 pMin = spriteRenderer.transform.TransformPoint(croppedRect.min);
        Vector3 pMax = spriteRenderer.transform.TransformPoint(croppedRect.max);

        return pMax;
    }
    public static Vector3 GetCroppedTopLeft(this SpriteRenderer spriteRenderer)
    {
        Rect croppedRect = new Rect(
          (spriteRenderer.sprite.textureRectOffset.x - spriteRenderer.sprite.rect.width / 2f) / spriteRenderer.sprite.pixelsPerUnit,
          (spriteRenderer.sprite.textureRectOffset.y - spriteRenderer.sprite.rect.height / 2f) / spriteRenderer.sprite.pixelsPerUnit,
          spriteRenderer.sprite.textureRect.width / spriteRenderer.sprite.pixelsPerUnit,
          spriteRenderer.sprite.textureRect.height / spriteRenderer.sprite.pixelsPerUnit);

        Vector3 pMin = spriteRenderer.transform.TransformPoint(croppedRect.min);
        Vector3 pMax = spriteRenderer.transform.TransformPoint(croppedRect.max);

        return new Vector3(pMin.x, pMax.y);
    }
    public static Vector3 GetCroppedBotLeft(this SpriteRenderer spriteRenderer)
    {
        Rect croppedRect = new Rect(
          (spriteRenderer.sprite.textureRectOffset.x - spriteRenderer.sprite.rect.width / 2f) / spriteRenderer.sprite.pixelsPerUnit,
          (spriteRenderer.sprite.textureRectOffset.y - spriteRenderer.sprite.rect.height / 2f) / spriteRenderer.sprite.pixelsPerUnit,
          spriteRenderer.sprite.textureRect.width / spriteRenderer.sprite.pixelsPerUnit,
          spriteRenderer.sprite.textureRect.height / spriteRenderer.sprite.pixelsPerUnit);

        Vector3 pMin = spriteRenderer.transform.TransformPoint(croppedRect.min);
        Vector3 pMax = spriteRenderer.transform.TransformPoint(croppedRect.max);

        return pMin;
    }
    public static Vector3 GetCroppedBotRight(this SpriteRenderer spriteRenderer)
    {
        Rect croppedRect = new Rect(
          (spriteRenderer.sprite.textureRectOffset.x - spriteRenderer.sprite.rect.width / 2f) / spriteRenderer.sprite.pixelsPerUnit,
          (spriteRenderer.sprite.textureRectOffset.y - spriteRenderer.sprite.rect.height / 2f) / spriteRenderer.sprite.pixelsPerUnit,
          spriteRenderer.sprite.textureRect.width / spriteRenderer.sprite.pixelsPerUnit,
          spriteRenderer.sprite.textureRect.height / spriteRenderer.sprite.pixelsPerUnit);

        Vector3 pMin = spriteRenderer.transform.TransformPoint(croppedRect.min);
        Vector3 pMax = spriteRenderer.transform.TransformPoint(croppedRect.max);

        return new Vector3(pMax.x, pMin.y);
    }
    #endregion

    #region Transform Extensions
    #region Position
    public static void SetZeroPosition(this Transform tr)
    {
        tr.position = Vector3.zero;
    }
    public static void SetZeroLocalPosition(this Transform tr)
    {
        tr.localPosition = Vector3.zero;
    }
    public static void SetXYZ(this Transform transform, float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }
    public static void SetLocalXYZ(this Transform transform, float x, float y, float z)
    {
        transform.localPosition = new Vector3(x, y, z);
    }
    public static void SetXY(this Transform transform, float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }
    public static void SetXY(this Transform transform, Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
    public static void SetLocalXY(this Transform transform, float x, float y)
    {
        transform.localPosition = new Vector3(x, y, transform.localPosition.z);
    }
    public static void SetLocalXY(this Transform transform, Vector3 pos)
    {
        transform.localPosition = new Vector3(pos.x, pos.y, transform.localPosition.z);
    }
    public static void SetXZ(this Transform transform, float x, float z)
    {
        transform.position = new Vector3(x, transform.position.y, z);
    }
    public static void SetXZ(this Transform transform, Vector3 pos)
    {
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
    }
    public static void SetLocalXZ(this Transform transform, float x, float z)
    {
        transform.localPosition = new Vector3(x, transform.localPosition.y, z);
    }
    public static void SetLocalXZ(this Transform transform, Vector3 pos)
    {
        transform.localPosition = new Vector3(pos.x, transform.localPosition.y, pos.z);
    }
    public static void SetYZ(this Transform transform, float y, float z)
    {
        transform.position = new Vector3(transform.position.x, y, z);
    }
    public static void SetYZ(this Transform transform, Vector3 pos)
    {
        transform.position = new Vector3(transform.position.x, pos.y, pos.z);
    }
    public static void SetLocalYZ(this Transform transform, float y, float z)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, y, z);
    }
    public static void SetLocalYZ(this Transform transform, Vector3 pos)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, pos.y, pos.z);
    }
    public static void SetX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
    public static void SetLocalX(this Transform transform, float x)
    {
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }
    public static void SetY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
    public static void SetLocalY(this Transform transform, float y)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }
    public static void SetZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
    public static void SetLocalZ(this Transform transform, float z)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
    }
    public static void IncX(this Transform transform, float dx)
    {
        SetX(transform, transform.position.x + dx);
    }
    public static void IncLocalX(this Transform transform, float dx)
    {
        SetLocalX(transform, transform.localPosition.x + dx);
    }
    public static void IncY(this Transform transform, float dy)
    {
        SetY(transform, transform.position.y + dy);
    }
    public static void IncLocalY(this Transform transform, float dy)
    {
        SetLocalY(transform, transform.localPosition.y + dy);
    }
    public static void IncZ(this Transform transform, float dz)
    {
        SetZ(transform, transform.position.z + dz);
    }
    public static void IncLocalZ(this Transform transform, float dz)
    {
        SetLocalZ(transform, transform.localPosition.z + dz);
    }
    #endregion
    #region Scale
    public static void SetScale(this Transform transform, float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
    public static void SetScaleX(this Transform transform, float scaleX)
    {
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }
    public static void SetScaleY(this Transform transform, float scaleY)
    {
        transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
    }
    public static void SetScaleZ(this Transform transform, float scaleZ)
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, scaleZ);
    }
    #endregion
    #region Rotation
    public static void SetRotation(this Transform transform, float angle)
    {
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.forward, angle);
    }
    public static void SetZeroRotation(this Transform tr)
    {
        tr.rotation = Quaternion.identity;
    }
    public static void SetZeroLocalRotation(this Transform tr)
    {
        tr.localRotation = Quaternion.identity;
    }
    #endregion
    #region Common
    public static void DestroyChilds(this Transform transform)
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            UnityEngine.Object.Destroy(transform.GetChild(0).gameObject);
        }
    }
    public static void SetActive(this Transform tr, bool state)
    {
        tr.gameObject.SetActive(state);
    }
    public static void SetNullParent(this Transform tr)
    {
        tr.SetParent(null);
    }    
    public static void SetZeroPosAndRot(this Transform tr)
    {
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
    }
    public static void SetZeroLocalPosAndRot(this Transform tr)
    {
        tr.localPosition = Vector3.zero;
        tr.localRotation = Quaternion.identity;
    }    
    #endregion
    #endregion
    
    #region IEnumerable Extensions
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (T item in items)
            action(item);
    }
    #endregion
    
    #region ParticleSystem Extensions
    public static IEnumerator PlayAndWaitForFinish(this ParticleSystem particleSystem)
    {
        particleSystem.Play();
        while (particleSystem.isPlaying)
            yield return null;
    }
    #endregion
    
    #region RectTransform Extensions

    #region Left, Right, Top, Bottom
    public static void SetLeft(this RectTransform rectTransform, float left)
    {
        rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
    }
    public static float GetLeft(this RectTransform rectTransform)
    {
        return rectTransform.offsetMin.x;
    }
    public static void SetRight(this RectTransform rectTransform, float right)
    {
        rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
    }
    public static float GetRight(this RectTransform rectTransform)
    {
        return -rectTransform.offsetMax.x;
    }
    public static void SetTop(this RectTransform rectTransform, float top)
    {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
    }
    public static float GetTop(this RectTransform rectTransform)
    {
        return -rectTransform.offsetMax.y;
    }
    public static void SetBottom(this RectTransform rectTransform, float bottom)
    {
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
    }
    public static float GetBottom(this RectTransform rectTransform)
    {
        return rectTransform.offsetMin.y;
    }
    public static void SetLeftTopRightBottom(this RectTransform rectTransform, float left, float top, float right, float bottom)
    {
        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(-right, -top);
    }
    #endregion

    #region PosX, PosY, Width, Height
    public static void SetPosX(this RectTransform rectTransform, float posX)
    {
        rectTransform.anchoredPosition = new Vector2(posX, rectTransform.anchoredPosition.y);
    }
    public static void SetPosY(this RectTransform rectTransform, float posY)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY);
    }
    public static void SetPosXY(this RectTransform rectTransform, float posX, float posY)
    {
        rectTransform.anchoredPosition = new Vector2(posX, posY);
    }
    public static void SetWidth(this RectTransform rectTransform, float width)
    {
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
    }
    public static float GetWidth(this RectTransform rectTransform)
    {
        return rectTransform.rect.width;
    }
    public static void SetHeight(this RectTransform rectTransform, float height)
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }
    public static float GetHeight(this RectTransform rectTransform)
    {
        return rectTransform.rect.height;
    }
    public static void SetWidthHeight(this RectTransform rectTransform, float width, float height)
    {
        rectTransform.sizeDelta = new Vector2(width, height);
    }
    public static void SetPosAndSize(this RectTransform rectTransform, float posX, float posY, float width, float height)
    {
        rectTransform.anchoredPosition = new Vector2(posX, posY);
        rectTransform.sizeDelta = new Vector2(width, height);
    }
    #endregion

    #region Anchor Offset
    public static void SetFullStrech(this RectTransform rectTransform)
    {
        rectTransform.SetLeft(0);
        rectTransform.SetTop(0);
        rectTransform.SetRight(0);
        rectTransform.SetBottom(0);
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5F, 0.5F);
    }
    public static void SetLeftAnchorOffset(this RectTransform rectTransform, float leftPercent)
    {
        rectTransform.anchorMin = new Vector2(leftPercent, rectTransform.anchorMin.y);
    }
    public static void SetRightAnchorOffset(this RectTransform rectTransform, float rightPercent)
    {
        rectTransform.anchorMax = new Vector2(1f - rightPercent, rectTransform.anchorMax.y);
    }
    public static void SetTopAnchorOffset(this RectTransform rectTransform, float topPercent)
    {
        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f - topPercent);
    }
    public static void SetBottomAnchorOffset(this RectTransform rectTransform, float bottomPercent)
    {
        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, bottomPercent);
    }
    public static void SetAnchorOffset(this RectTransform rectTransform, float left, float top, float right, float bottom)
    {
        rectTransform.anchorMin = new Vector2(left, bottom);
        rectTransform.anchorMax = new Vector2(1f - right, 1f - top);
    }
    #endregion

    #region World positions

    private static readonly Vector3[] _fourCorners = new Vector3[4];//start bottom left and clockwise
    public static Vector2 GetWorldCenter(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return new Vector2((_fourCorners[0].x + _fourCorners[3].x) / 2f, (_fourCorners[0].y + _fourCorners[1].y) / 2f);
    }
    public static float GetWorldLeft(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return _fourCorners[0].x;
    }
    public static float GetWorldRight(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return _fourCorners[2].x;
    }
    public static float GetWorldTop(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return _fourCorners[1].y;
    }
    public static float GetWorldBottom(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return _fourCorners[0].y;
    }
    public static Vector2 GetWorldTopLeft(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return new Vector2(_fourCorners[0].x, _fourCorners[1].y);
    }
    public static Vector2 GetWorldTopRight(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return new Vector2(_fourCorners[2].x, _fourCorners[1].y);
    }
    public static Vector2 GetWorldBottomLeft(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return new Vector2(_fourCorners[0].x, _fourCorners[0].y);
    }
    public static Vector2 GetWorldBottomRight(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return new Vector2(_fourCorners[2].x, _fourCorners[0].y);
    }
    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_fourCorners);
        return new Rect(_fourCorners[0].x, _fourCorners[0].y, Mathf.Abs(_fourCorners[3].x - _fourCorners[0].x), Mathf.Abs(_fourCorners[1].y - _fourCorners[0].y));
    }
    #endregion

    #endregion
}