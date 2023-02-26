using Demo_MoveMotor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public class CharacterMotor_Physic : CharacterMotor
    {
        /// <summary>
        /// ǰ���Ӵ�ǽ�ķ��߷���
        /// </summary>
        protected Vector3 m_wallHitNormal;
        /// <summary>
        /// ǰ��/����Ӵ�ǽ�ı�Ե��
        /// </summary>
        protected Vector3 m_wallHitEdge;
        /// <summary>
        /// ��ż�
        /// </summary>
        [SerializeField] protected Transform m_leftFootTran;
        /// <summary>
        /// �ҽż�
        /// </summary>
        [SerializeField] protected Transform m_rightFootTran;
        /// <summary>
        /// �Ų�ǰ���ϵ
        /// </summary>
        protected float m_footstep = -1f;
        /// <summary>
        /// ��Խ���Բ���뾶
        /// </summary>
        [SerializeField, Header("���Բ���뾶")] private float m_capsuleCastRadius = 0.2f;
        /// <summary>
        /// ��Խ������
        /// </summary>
        [SerializeField, Header("��Խ������")] private float m_capsuleCastDistance = 1.2f;
        /// <summary>
        /// ��Խ������߶�
        /// </summary>
        [SerializeField, Header("��Խ������߶�")] private float m_maxVaultHeight = 1.5f;
        /// <summary>
        /// ��Խ��λ�þ���
        /// </summary>
        [SerializeField, Header("��Խ��λ�þ���")] private float m_distanceAfterVault = 0.5f;
        [Space]
        /// <summary>
        /// ���뾶
        /// </summary>
        [SerializeField, Header("���뾶")] private float m_overlapRadius = 0.75f;
        /// <summary>
        /// ���߶�
        /// </summary>
        [SerializeField, Header("���߶�")] private float m_capsuleCastHeight = 1f;
        /// <summary>
        /// ��λ������С�߶�
        /// </summary>
        [SerializeField, Header("������С�߶�")] private float m_minClimbHeight = 0.5f;
        /// <summary>
        /// ��λ�������߶�
        /// </summary>
        [SerializeField, Header("��λ�������߶�")] private float m_maxClimbHeightShort = 1.5f;
        /// <summary>
        /// ��λλ�������߶�
        /// </summary>
        [SerializeField, Header("��λλ�������߶�")] private float m_maxClimbHeightHeight = 2.5f;
        /// <summary>
        /// ��ǰ������λ���ֵ�λ�ã�
        /// </summary>
        protected Vector3 m_currentClimbPoint;

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
            CalculateRelativityTarget();
            CalculateAngularVelocity();
            CalculateFootStep();
            CalculateGravity();
            CalculateGround();
        }

        protected void CalculateRelativityTarget()
        {
            m_relativityForward = Vector3.Dot(m_targetDirection, rootTransform.forward);
            m_relativityRight = Vector3.Dot(m_targetDirection, rootTransform.right);

            if (m_isClimbing)
            {
                if (m_relativityRight >= 0.5f && !CalculateLege(1))
                    m_relativityRight = 0f;

                if (m_relativityRight <= -0.5f && !CalculateLege(-1))
                    m_relativityRight = 0f;

            }
        }

        /// <summary>
        /// ���½��ٶ�
        /// </summary>
        protected void CalculateAngularVelocity()
        {
            Vector3 direction = m_targetDirection;
            direction.y = 0f;
            Vector3 roleDelta = rootTransform.InverseTransformDirection(direction);
            //����Ŀ��Ƕ��뵱ǰ�Ƕȵļнǻ���
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
        /// �������
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
        /// ������
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
        /// �������Ŀ��
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
        /// ��ⷭԽ��
        /// </summary>
        public bool CalculateVault()
        {

            Vector3 p1 = rootTransform.position + rootTransform.forward * m_capsuleCastDistance + Vector3.up * m_capsuleCastRadius;
            Vector3 p2 = rootTransform.position + rootTransform.forward * m_capsuleCastDistance + Vector3.up * (m_maxVaultHeight - m_capsuleCastRadius);

            m_debugHelper.DrawCapsule(p1, p2, m_capsuleCastRadius, Color.white);
            m_debugHelper.DrawLabel("��Խ���", p1 + Vector3.up, Color.blue);

            //��ⳤ��
            if (Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, -rootTransform.forward, out RaycastHit capsuleHit, m_capsuleCastDistance, m_vaultLayer, QueryTriggerInteraction.Ignore))
            {
                Vector3 startTop = capsuleHit.point;
                startTop.y = rootTransform.position.y + m_maxVaultHeight + m_capsuleCastRadius;
                m_debugHelper.DrawSphere(capsuleHit.point, m_capsuleCastRadius, Color.yellow, 1f);

                //���߶�
                if (Physics.SphereCast(startTop, m_capsuleCastRadius, Vector3.down, out RaycastHit top, m_maxVaultHeight, m_vaultLayer, QueryTriggerInteraction.Ignore))
                {
                    capsuleHit.normal = new Vector3(capsuleHit.normal.x, 0f, capsuleHit.normal.z);
                    capsuleHit.normal.Normalize();

                    //���Ŀ���ĸ߶�
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
        /// �������
        /// </summary>
        /// <returns></returns>
        public bool CalculateClimb(out int climbType)
        {
            Vector3 p1 = rootTransform.position + Vector3.up * (m_minClimbHeight + m_capsuleCastRadius);
            Vector3 p2 = rootTransform.position + Vector3.up * (m_capsuleCastHeight - m_capsuleCastRadius);
            m_debugHelper.DrawCapsule(p1, p2, m_overlapRadius, Color.white);
            m_debugHelper.DrawLabel("�������", p1 + Vector3.up, Color.blue);
            if(Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, rootTransform.forward, out RaycastHit forwardHit, m_overlapRadius, m_climbLayer, QueryTriggerInteraction.Ignore))
            {
                RaycastHit topHit;
                Vector3 sphereStart = forwardHit.point;
                //�ȼ���λ����
                sphereStart.y = transform.position.y + m_maxClimbHeightShort + m_capsuleCastRadius;
                if (Physics.SphereCast(sphereStart, m_capsuleCastRadius, Vector3.down, out topHit, m_maxClimbHeightShort - m_minClimbHeight, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    m_debugHelper.DrawSphere(topHit.point, 0.1f, Color.red, 3f);
                    m_stateInfos.AddMatchTargetList(new List<Vector3>() { topHit.point });
                    climbType = 1;
                    return true;
                }
                //�ټ���λ����
                sphereStart.y = rootTransform.position.y + m_maxClimbHeightHeight + m_capsuleCastRadius;
                if (Physics.SphereCast(sphereStart, m_capsuleCastRadius, Vector3.down, out topHit, m_maxClimbHeightHeight - m_maxClimbHeightShort, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    m_debugHelper.DrawSphere(topHit.point, 0.1f, Color.red, 3f);
                    m_stateInfos.AddMatchTargetList(new List<Vector3>() { topHit.point });
                    //��¼������
                    m_currentClimbPoint = topHit.point;
                    //����ǽ��
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
        /// ��Ե����
        /// </summary>
        /// <returns></returns>
        public bool CalculateLege(int direction)
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
        /// ������ҽ�
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
