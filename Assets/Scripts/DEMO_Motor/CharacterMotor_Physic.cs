using Demo_MoveMotor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public partial class CharacterMotor
    {
        private float m_footstep = -1f;

        public void CalculateGravity()
        {
            if (!characterController.enabled)
            {
                verticalSpeed = 0f;
                return;
            }
            float damping = UpdateAirDamping();
            verticalSpeed = isGround && verticalSpeed <= 0f ? 0f : verticalSpeed + (damping + m_gravity) * Time.deltaTime;
        }

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

        public void CalculateFootStep()
        {
            Vector3 localForward = transform.TransformPoint(Vector3.forward);
            float left = Vector3.Dot(localForward, m_leftFootTran.position);
            float right = Vector3.Dot(localForward, m_rightFootTran.position);
            m_footstep = left > right ? -1f : 1f;
            //animator.SetFloat(Float_Footstep_Hash, left > right ? -1f : 1f);

#if UNITY_EDITOR
            Debug.DrawLine(localForward, m_leftFootTran.position, Color.green);
            Debug.DrawLine(localForward, m_rightFootTran.position, Color.green);
#endif
        }
    }
}
