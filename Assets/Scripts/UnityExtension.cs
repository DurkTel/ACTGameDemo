using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
