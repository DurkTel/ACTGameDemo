using Demo_MoveMotor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Demo_MoveMotor
{
    public class CharacterMotor_Physic : CharacterMotor
    {
        public enum DirectionCast
        {
            Left = -1,
            Right = 1,
            Forward = 2,
            Backward = -2,
            Up = 3,
            Down = -3,
        }
        /// <summary>
        /// 前方接触墙的法线方向
        /// </summary>
        protected Vector3 m_wallHitNormal;
        /// <summary>
        /// 前方/侧面接触墙的边缘点
        /// </summary>
        protected Vector3 m_wallHitEdge;
        /// <summary>
        /// 左脚尖
        /// </summary>
        [SerializeField] protected Transform m_leftFootTran;
        /// <summary>
        /// 右脚尖
        /// </summary>
        [SerializeField] protected Transform m_rightFootTran;
        /// <summary>
        /// 脚步前后关系
        /// </summary>
        protected float m_footstep = -1f;
        /// <summary>
        /// 检测圆柱半径
        /// </summary>
        [SerializeField, Header("检测圆柱半径")] private float m_capsuleCastRadius = 0.2f;
        /// <summary>
        /// 翻越检测距离
        /// </summary>
        [SerializeField, Header("翻越检测距离")] private float m_capsuleCastDistance = 1.2f;
        /// <summary>
        /// 翻越检测最大高度
        /// </summary>
        [SerializeField, Header("翻越检测最大高度")] private float m_maxVaultHeight = 1.5f;
        /// <summary>
        /// 翻越后位置距离
        /// </summary>
        [SerializeField, Header("翻越后位置距离")] private float m_distanceAfterVault = 0.5f;
        [Space]
        /// <summary>
        /// 检测半径
        /// </summary>
        [SerializeField, Header("检测半径")] private float m_overlapRadius = 0.75f;
        /// <summary>
        /// 检测高度
        /// </summary>
        [SerializeField, Header("检测高度")] private float m_capsuleCastHeight = 1f;
        /// <summary>
        /// 低位攀爬最小高度
        /// </summary>
        [SerializeField, Header("攀爬最小高度")] private float m_minClimbHeight = 0.5f;
        /// <summary>
        /// 低位攀爬最大高度
        /// </summary>
        [SerializeField, Header("低位攀爬最大高度")] private float m_maxClimbHeightShort = 1.5f;
        /// <summary>
        /// 高位位攀爬最大高度
        /// </summary>
        [SerializeField, Header("高位位攀爬最大高度")] private float m_maxClimbHeightHeight = 2.5f;
        /// <summary>
        /// 当前攀爬点位（手的位置）
        /// </summary>
        protected Vector3 m_currentClimbPoint;

        protected Vector3 m_wallRunForward;

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            CalculateAngularVelocity();
            CalculateFootStep();
            CalculateGravity();
            CalculateGround();
        }

        /// <summary>
        /// 计算输入相对角色的方向
        /// </summary>
        protected void CalculateRelativityTarget(Vector3 input)
        {
            m_relativityForward = Vector3.Dot(input.normalized, rootTransform.forward);
            m_relativityRight = Vector3.Dot(input.normalized, rootTransform.right);
            m_relativityForward = m_relativityForward.NormalizeFloat();
            m_relativityRight = m_relativityRight.NormalizeFloat();

            if (m_isClimbing)
            {
                if (m_relativityRight >= 0.5f && !CalculateEdge(1))
                    m_relativityRight = 0f;

                if (m_relativityRight <= -0.5f && !CalculateEdge(-1))
                    m_relativityRight = 0f;

            }
        }

        /// <summary>
        /// 更新角速度
        /// </summary>
        protected void CalculateAngularVelocity()
        {
            Vector3 direction = m_targetDirection;
            direction.y = 0f;
            Vector3 roleDelta = rootTransform.InverseTransformDirection(direction);
            //计算目标角度与当前角度的夹角弧度
            float deg1 = Mathf.Atan2(roleDelta.x, roleDelta.z) * Mathf.Rad2Deg;
            m_targetDeg = deg1;
            deg1 *= 0.002f;

            Vector3 localForward = rootTransform.InverseTransformDirection(m_lastForward);
            float deg2 = Mathf.Atan2(localForward.x, localForward.z) * Mathf.Rad2Deg;

            m_lastForward = rootTransform.forward;
            m_angularVelocity = deg1 - deg2;
            m_angularVelocity *= 0.002f;
            m_angularVelocity = Mathf.Clamp(m_angularVelocity / Time.fixedDeltaTime, -1f, 1f);
        }

        /// <summary>
        /// 检测重力
        /// </summary>
        public void CalculateGravity()
        {
            if (!characterController.enabled || m_isClimbing)
            {
                verticalSpeed = 0f;
                return;
            }
            verticalSpeed = isGround && verticalSpeed <= 0f ? 0f : verticalSpeed + m_gravity * Time.fixedDeltaTime;
        }

        /// <summary>
        /// 检测地面
        /// </summary>
        public void CalculateGround()
        {
            if (Physics.SphereCast(rootTransform.position + Vector3.up * 0.5f, characterController.radius,
                Vector3.down, out RaycastHit hitInfo, 0.5f - characterController.radius + characterController.skinWidth * 2, m_groundLayer))
            {
                m_jumpCount = 0;
                isGround = true;
            }
            else
            {
                isGround = false;
            }
            isFall = !Physics.SphereCast(rootTransform.position + Vector3.up * 0.5f, characterController.radius, Vector3.down, out RaycastHit hit, 0.8f, m_groundLayer);

        }

        /// <summary>
        /// 检测锁定目标
        /// </summary>
        public void CalculateLockon()
        {
            if(!m_isGazing)
            {
                m_camera.lockon = null;
                return;
            }
            Collider[] colliders = Physics.OverlapSphere(rootTransform.position, m_lockonRadius, m_lockonLayer);
            Transform newLock = null;
            float minDis = -1f;
            Vector2 pos1 = Vector2.zero;
            Vector2 pos2 = new Vector2(rootTransform.position.x, rootTransform.position.z);
            foreach (Collider coll in colliders)
            {
                pos1.Set(coll.transform.position.x, coll.transform.position.z);
                float dis = Vector2.Distance(pos1, pos2);
                if (minDis < 0 || dis < minDis)
                {
                    minDis = dis;
                    newLock = coll.transform;
                }
            }

            m_camera.lockon = newLock;
        }

        /// <summary>
        /// 检测翻越物
        /// </summary>
        public bool CalculateVault()
        {

            Vector3 p1 = rootTransform.position + rootTransform.forward * m_capsuleCastDistance + Vector3.up * m_capsuleCastRadius;
            Vector3 p2 = rootTransform.position + rootTransform.forward * m_capsuleCastDistance + Vector3.up * (m_maxVaultHeight - m_capsuleCastRadius);

            //m_debugHelper.DrawCapsule(p1, p2, m_capsuleCastRadius, Color.white);
            //m_debugHelper.DrawLabel("翻越检测", p1 + Vector3.up, Color.blue);

            //检测长度
            if (Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, -rootTransform.forward, out RaycastHit capsuleHit, m_capsuleCastDistance, m_vaultLayer, QueryTriggerInteraction.Ignore))
            {
                Vector3 startTop = capsuleHit.point;
                startTop.y = rootTransform.position.y + m_maxVaultHeight + m_capsuleCastRadius;
                m_debugHelper.DrawSphere(capsuleHit.point, m_capsuleCastRadius, Color.yellow, 1f);

                //检测高度
                if (Physics.SphereCast(startTop, m_capsuleCastRadius, Vector3.down, out RaycastHit top, m_maxVaultHeight, m_vaultLayer, QueryTriggerInteraction.Ignore))
                {
                    capsuleHit.normal = new Vector3(capsuleHit.normal.x, 0f, capsuleHit.normal.z);
                    capsuleHit.normal.Normalize();

                    //检测目标点的高度
                    Vector3 targetPosition = capsuleHit.point + capsuleHit.normal * m_distanceAfterVault;
                    if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit groundHit, 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                        targetPosition.y = groundHit.point.y;
                    else
                        targetPosition.y = rootTransform.position.y;


                    m_debugHelper.DrawSphere(top.point, m_capsuleCastRadius, Color.blue, 1f);
                    if (Physics.Raycast(rootTransform.position, rootTransform.forward, out RaycastHit hit, m_capsuleCastDistance, m_vaultLayer, QueryTriggerInteraction.Ignore))
                    {
                        Vector3 hitPoint = new Vector3(hit.point.x, top.point.y, hit.point.z);
                        m_stateInfos.AddMatchTargetList(new List<Vector3>() { hitPoint, targetPosition });

                        m_debugHelper.DrawSphere(hitPoint, 0.1f, Color.red, 1f);
                        m_debugHelper.DrawSphere(targetPosition, 0.1f, Color.red, 1f);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 检测攀爬
        /// </summary>
        /// <returns></returns>
        public bool CalculateClimb(out int climbType)
        {
            Vector3 p1 = rootTransform.position + Vector3.up * (m_minClimbHeight + m_capsuleCastRadius);
            Vector3 p2 = rootTransform.position + Vector3.up * (m_capsuleCastHeight - m_capsuleCastRadius);
            //m_debugHelper.DrawCapsule(p1, p2, m_overlapRadius, Color.white);
            //m_debugHelper.DrawLabel("攀爬检测", p1 + Vector3.up, Color.blue);
            if(Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, rootTransform.forward, out RaycastHit forwardHit, m_overlapRadius, m_climbLayer, QueryTriggerInteraction.Ignore))
            {
                RaycastHit topHit;
                Vector3 sphereStart = forwardHit.point;
                //先检测低位攀爬
                sphereStart.y = transform.position.y + m_maxClimbHeightShort + m_capsuleCastRadius;
                if (Physics.SphereCast(sphereStart, m_capsuleCastRadius, Vector3.down, out topHit, m_maxClimbHeightShort - m_minClimbHeight, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    m_debugHelper.DrawSphere(topHit.point, 0.1f, Color.red, 3f);
                    m_stateInfos.AddMatchTargetList(new List<Vector3>() { topHit.point });
                    climbType = 1;
                    return true;
                }
                //再检测高位攀爬
                sphereStart.y = rootTransform.position.y + m_maxClimbHeightHeight + m_capsuleCastRadius;
                if (Physics.SphereCast(sphereStart, m_capsuleCastRadius, Vector3.down, out topHit, m_maxClimbHeightHeight - m_maxClimbHeightShort, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    m_debugHelper.DrawSphere(topHit.point, 0.1f, Color.red, 3f);
                    m_stateInfos.AddMatchTargetList(new List<Vector3>() { topHit.point });
                    //记录攀爬点
                    m_currentClimbPoint = topHit.point;
                    //面向墙壁
                    Vector3 faceTo = -forwardHit.normal;
                    faceTo.y = 0f;
                    rootTransform.rotation = Quaternion.LookRotation(faceTo);
                    climbType = 2;
                    return true;
                }
            }

            climbType = 0;
            return false;
        }

        /// <summary>
        /// 边缘点检测
        /// </summary>
        /// <returns></returns>
        public bool CalculateEdge(int direction)
        {
            int count = 3;
            int missCount = 0;
            Vector3 p = rootTransform.position;
            p.y = m_currentClimbPoint.y;
            Collider [] colls;
            
            for (int i = 1; i <= count; i++)
            {
                Vector3 center = p + rootTransform.right * i * direction * 0.35f * 2f;
                Vector3 top = center + Vector3.up * 0.25f;
                Vector3 down = center + Vector3.down * 0.25f;

                colls = Physics.OverlapCapsule(top, down, 0.35f, m_climbLayer);
                Color color = colls.Length > 0 ? Color.green : Color.red;

                if (colls.Length == 0)
                    missCount++;

                m_debugHelper.DrawCapsule(top, down, 0.35f, color);
            }

            return count - missCount > 0;
        }

        /// <summary>
        /// 检测跳跃攀爬点
        /// </summary>
        /// <param name="direction">检测方向：上下左右后 2 -2 -1 1 0</param>
        /// <returns></returns>
        public bool CalculateJumpClimb(DirectionCast direction, out Vector3 targetPosition, out Quaternion targetQuaternion)
        {
            //悬挂时手的位置
            Vector3 handPos = rootTransform.position + Vector3.up * 1.5f;

            //跳跃攀爬点的检测范围
            Vector3 p1 = handPos + Vector3.up * m_maxClimbHeightHeight;
            Vector3 p2 = handPos + Vector3.down * m_maxClimbHeightHeight;
            m_debugHelper.DrawCapsule(p1 - rootTransform.forward * 4.5f, p2 - rootTransform.forward * 4.5f, m_capsuleCastRadius, Color.blue);
            if (direction == DirectionCast.Backward)
            {
                if (Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, -rootTransform.forward, out RaycastHit horizontal, 4.5f, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    float height = horizontal.collider.bounds.size.y;
                     if (Physics.SphereCast(horizontal.point + Vector3.up * height, m_capsuleCastRadius, Vector3.down, out RaycastHit topHit, height, m_climbLayer, QueryTriggerInteraction.Ignore))
                    {
                        m_debugHelper.DrawSphere(topHit.point + Vector3.down * 1.77f + horizontal.normal * 0.1f, 0.1f, Color.red, 3f);
                        targetPosition = topHit.point + Vector3.down * 1.77f + horizontal.normal * 0.1f;
                        targetQuaternion = Quaternion.LookRotation(-horizontal.normal.normalized);
                        return true;
                    }
                }
            }

            targetPosition = rootTransform.position;
            targetQuaternion = rootTransform.rotation;

            return false;
        }

        /// <summary>
        /// 检测墙跑
        /// </summary>
        /// <returns></returns>
        public bool CalculateWallRun(out RaycastHit wallHit, out float dir)
        {
            //左右是否有墙
            bool right = Physics.SphereCast(rootTransform.position + Vector3.up, m_capsuleCastRadius, rootTransform.right, out RaycastHit rightHit, 0.5f, m_wallRunLayer, QueryTriggerInteraction.Ignore);
            bool left = Physics.SphereCast(rootTransform.position + Vector3.up, m_capsuleCastRadius, -rootTransform.right, out RaycastHit leftHit, 0.5f, m_wallRunLayer, QueryTriggerInteraction.Ignore);

            //m_debugHelper.DrawLine(rootTransform.position + Vector3.up, rootTransform.position + Vector3.up + rootTransform.right * 0.5f, Color.blue);
            //m_debugHelper.DrawLine(rootTransform.position + Vector3.up, rootTransform.position + Vector3.up - rootTransform.right * 0.5f, Color.blue);
            if (right || left)
            {
                //高度是否足够
                //if (!Physics.Raycast(rootTransform.position, Vector3.down, 1f, m_groundLayer))
                //{
                    wallHit = right ? rightHit : leftHit;
                    dir = right ? 1f : -1f;
                    return true;
                //}
            }

            dir = 0f;
            wallHit = default(RaycastHit);
            return false;
        }

        /// <summary>
        /// 检测左右脚
        /// </summary>
        public void CalculateFootStep()
        {
            Vector3 localForward = transform.TransformPoint(Vector3.forward);
            float left = Vector3.Dot(localForward, m_leftFootTran.position);
            float right = Vector3.Dot(localForward, m_rightFootTran.position);
            m_footstep = left > right ? -1f : 1f;

        }

    }

}
