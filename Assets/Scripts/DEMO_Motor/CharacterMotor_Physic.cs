using Demo_MoveMotor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
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
            if (!characterController.enabled)
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

        public bool ClimbCondition()
        {
            if (Physics.Raycast(rootTransform.position + Vector3.up + Vector3.up * 0.5f, rootTransform.forward, out RaycastHit obsHit, 1f, m_climbLayer))
            {
                //ǽ��ķ��߷���
                m_wallHitNormal = obsHit.normal;
                Vector3 target = obsHit.point;
                //ǽ�����߶�
                target.y = obsHit.collider.bounds.size.y;
                if (Physics.Raycast(target + Vector3.up, Vector3.down, out RaycastHit wallHit, obsHit.collider.bounds.size.y + 1f, m_climbLayer))
                {
                    m_wallHitEdge = wallHit.point;
                    if (m_wallHitEdge.y <= m_maxClimbHeight)
                        return true;
                }
            }

            return false;
        }

        [SerializeField] private float capsuleCastRadius = 0.2f;
        [SerializeField] private float capsuleCastDistance = 1.2f;
        [Space]
        [SerializeField] private float maxVaultHeight = 1.5f;
        [SerializeField] private float distanceAfterVault = 0.5f;
        /// <summary>
        /// ��ⷭԽ��
        /// </summary>
        public bool CalculateVault()
        {

            Vector3 p1 = rootTransform.position + rootTransform.forward * capsuleCastDistance + Vector3.up * capsuleCastRadius;
            Vector3 p2 = rootTransform.position + rootTransform.forward * capsuleCastDistance + Vector3.up * (maxVaultHeight - capsuleCastRadius);

            m_debugHelper.DrawCapsule(p1, p2, capsuleCastRadius, Color.white);
            m_debugHelper.DrawLabel("��Խ���", p1 + Vector3.up, Color.white);

            //��ⳤ��
            if (Physics.CapsuleCast(p1, p2, capsuleCastRadius, -rootTransform.forward, out RaycastHit capsuleHit, capsuleCastDistance, m_vaultLayer, QueryTriggerInteraction.Ignore))
            {
                Vector3 startTop = capsuleHit.point;
                startTop.y = rootTransform.position.y + maxVaultHeight + capsuleCastRadius;
                m_debugHelper.DrawSphere(capsuleHit.point, capsuleCastRadius, Color.yellow, 1f);

                //���߶�
                if (Physics.SphereCast(startTop, capsuleCastRadius, Vector3.down, out RaycastHit top, maxVaultHeight, m_vaultLayer, QueryTriggerInteraction.Ignore))
                {
                    capsuleHit.normal = new Vector3(capsuleHit.normal.x, 0f, capsuleHit.normal.z);
                    capsuleHit.normal.Normalize();

                    m_debugHelper.DrawSphere(top.point, capsuleCastRadius, Color.blue, 1f);
                    if (Physics.Raycast(rootTransform.position, rootTransform.forward, out RaycastHit hit, capsuleCastDistance, m_vaultLayer, QueryTriggerInteraction.Ignore))
                    {
                        m_stateInfos.matchTarget = new Vector3(hit.point.x, top.point.y - 0.8f, hit.point.z);
                        m_debugHelper.DrawSphere(m_stateInfos.matchTarget, 0.1f, Color.red, 1f);
                        characterController.enabled = false;
                        //rootTransform.position = m_stateInfos.matchTarget;
                        return true;
                    }
                }
            }

            return false;
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
