﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ToolExtensions
{
    /// <summary>
    /// 标准化角度
    /// </summary>
    /// <param name="eulerAngle"></param>
    /// <returns>返回-180到180的角度</returns>
    public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
    {
        var delta = eulerAngle;

        if (delta.x > 180)
        {
            delta.x -= 360;
        }
        else if (delta.x < -180)
        {
            delta.x += 360;
        }

        if (delta.y > 180)
        {
            delta.y -= 360;
        }
        else if (delta.y < -180)
        {
            delta.y += 360;
        }

        if (delta.z > 180)
        {
            delta.z -= 360;
        }
        else if (delta.z < -180)
        {
            delta.z += 360;
        }

        return new Vector3(delta.x, delta.y, delta.z);
    }

    /// <summary>
    /// 标准化浮点
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float NormalizeFloat(this float value)
    {
        if (value <= 0.1f && value >= -0.1f)
            value = 0f;

        if (value < 0f)
            value = -1f;

        if (value > 0f)
            value = 1f;

        return value;
    }

    /// <summary>
    /// 获取范围内最近的Transform
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform ObtainNearestTarget(this Transform transform, float radius, LayerMask layer, params Transform[] ignore)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layer);
        Transform target = null;
        float minDis = radius;
        Vector2 pos1 = Vector2.zero;
        Vector2 pos2 = new Vector2(transform.position.x, transform.position.z);
        foreach (Collider coll in colliders)
        {
            if (ignore.Length > 0 && Array.IndexOf<Transform>(ignore, coll.transform) >= 0)
                continue;
            pos1.Set(coll.transform.position.x, coll.transform.position.z);
            float dis = Vector2.Distance(pos1, pos2);
            if (dis < minDis)
            {
                minDis = dis;
                target = coll.transform;
            }
        }
        return target;
    }
    public static bool TryUniqueAdd<T>(this List<T> list, T item, Action<T> callBack = null)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            callBack?.Invoke(item);
            return true;
        }

        return false;
    }

    public static bool TryRemove<T>(this List<T> list, T item, Action<T> callBack = null)
    {
        if (list.Contains(item))
        {
            list.Remove(item);
            callBack?.Invoke(item);
            return true;
        }

        return false;
    }

    public static T TryAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.TryGetComponent(out T component))
        {
            return component;
        }

        return gameObject.AddComponent<T>();
    }

    public static Component TryAddComponent(this GameObject gameObject, string type)
    {
        Component component = gameObject.GetComponent(type);
        if (component != null)
            return component;

        component = gameObject.AddComponent(Type.GetType(type));

        return component;
    }

    public static Component TryAddComponent(this Transform transform, string type)
    {
        return TryAddComponent(transform.gameObject, type);
    }

    public static Component TryAddComponent(this GameObject gameObject, Type type)
    {
        Component component = gameObject.GetComponent(type);
        if (component != null)
            return component;

        component = gameObject.AddComponent(type);

        return component;
    }

    public static Component TryAddComponent(this Transform transform, Type type)
    {
        return TryAddComponent(transform.gameObject, type);
    }

    public static void SetActive(this Component component, bool value)
    {
        if (component != null && component.gameObject.activeSelf != value)
        {
            component.gameObject.SetActive(value);
        }
    }

    public static void SetParentIgnore(this Transform transform, Transform parent)
    {
        Vector3 oriPos = transform.localPosition;
        Vector3 oriRote = transform.localEulerAngles;
        Vector3 oriScale = transform.localScale;
        transform.SetParent(parent);
        transform.localPosition = oriPos;
        transform.localEulerAngles = oriRote;
        transform.localScale = oriScale;
    }

    public static void SetParentZero(this Transform transform, Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    public static void SetParentNew(this Transform transform, Transform parent, Vector3 pos)
    { 
        transform.SetParent(parent);
        transform.localPosition = pos;
    }

    public static bool IsNull(this UnityEngine.Object obj)
    {
        return obj == null || obj.Equals(null);
    }
}