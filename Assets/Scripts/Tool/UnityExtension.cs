using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public static class UnityExtension
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
}
