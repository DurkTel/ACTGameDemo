using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 轨道相机
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    /// <summary>
    /// 焦点
    /// </summary>
    [SerializeField]
    private Transform focus = default;
    /// <summary>
    /// 锁定
    /// </summary>
    public Transform lockon = null;
    /// <summary>
    /// 与焦点的距离
    /// </summary>
    [SerializeField, Range(1f, 20f)]
    private float distance = 5f;
    /// <summary>
    /// 焦点的缓动半径
    /// </summary>
    [SerializeField, Min(0f)]
    private float focusRadius = 1f;
    /// <summary>
    /// 焦点居中系数
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float focusCentering = 0.5f;
    /// <summary>
    /// 相机旋转速度
    /// </summary>
    [SerializeField, Range(1f, 360f)]
    private float rotationSpeed = 90f;
    /// <summary>
    /// 约束角度
    /// </summary>
    [SerializeField, Range(-89f, 89f)]
    private float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    /// <summary>
    /// 对齐平滑范围
    /// </summary>
    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;
    /// <summary>
    /// 自动对齐
    /// </summary>
    [SerializeField]
    private float alignDelay = 5f;
    /// <summary>
    /// 锁定时的平滑时间
    /// </summary>
    [SerializeField]
    private float lockonSmooth = 300f;
    /// <summary>
    /// 相机遮挡检测的层级
    /// </summary>
    [SerializeField]
    private LayerMask obstructionMask = -1;
    /// <summary>
    /// 最后一次收到旋转发生时间
    /// </summary>
    private float lastManualRotationTime;
    /// <summary>
    /// 焦点对象的现在/以前的位置
    /// </summary>
    private Vector3 focusPoint, previousFocusPoint;
    /// <summary>
    /// 摄像机轨道角
    /// </summary>
    private Vector2 orbitAngles = new Vector2(45f, 0f);
    /// <summary>
    /// 规则相机
    /// </summary>
    private Camera regularCamera;
    [SerializeField]
    private bool invertYAxis;
    [SerializeField]
    private bool invertXAxis;

    private Vector2 inputDelta;
    [SerializeField]
    private float lockonHeight;

    private Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }


    private void Awake()
    {
        regularCamera = GetComponent<Camera>();
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
        Cursor.lockState = CursorLockMode.Locked;   
    }

    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
            maxVerticalAngle = minVerticalAngle;
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        Quaternion lookRotation;
        if (LockonRotation() || ManualRotation() || AutoMaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
            lookRotation = transform.localRotation;

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection,
            out RaycastHit hit, lookRotation, castDistance, obstructionMask))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        if (lockon == null)
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        else
        {
            float delta = lockonSmooth * Time.deltaTime;
            transform.SetPositionAndRotation(Vector3.Slerp(transform.position, lookPosition, delta), Quaternion.Slerp(transform.rotation, lookRotation, delta));
        }

    }

    /// <summary>
    /// 更新焦点对象的位置
    /// </summary>
    private void UpdateFocusPoint()
    {
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float t = 1f;
            if (distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }
            //与上次相比 焦点的位移大于缓动半径才进行设值
            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
            focusPoint = targetPoint;
    }

    /// <summary>
    /// 控制相机旋转
    /// </summary>
    private bool ManualRotation()
    {
        if (lockon != null)
            return false;

        //输入误差
        const float e = 0.001f;
        if (inputDelta.x < -e || inputDelta.x > e || inputDelta.y < -e || inputDelta.y > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * inputDelta;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 锁定相机旋转
    /// </summary>
    private bool LockonRotation()
    {
        if (lockon == null)
            return false;

        Vector3 target = lockon.position - focus.position + Vector3.up * lockonHeight;
        orbitAngles = Quaternion.LookRotation(target).eulerAngles;
        return true;
    }

    /// <summary>
    /// 角度限制
    /// </summary>
    private void ConstrainAngles()
    {
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);
        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }

    /// <summary>
    /// 自动对齐
    /// </summary>
    /// <returns></returns>
    private bool AutoMaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay)
        {
            return false;
        }

        Vector2 movement = new Vector2(focusPoint.x - previousFocusPoint.x, focusPoint.z - previousFocusPoint.z);
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.0001f)
            return false;

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < alignSmoothRange)
            rotationChange *= deltaAbs / alignSmoothRange;
        else if (180f - deltaAbs < alignSmoothRange)
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    private static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    public void GetAxisInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (invertYAxis)
            input.y = -input.y;

        if (invertXAxis)
            input.x = -input.x;

        inputDelta.Set(input.y, input.x);
    }

}
