using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using UnityEditor;


namespace Demo_MoveMotor
{

    public partial class CharacterMotor : MonoBehaviour//, ICharacterControl
    {
        #region �ƶ�����
        [SerializeField, Header("������")]
        protected OrbitCamera m_camera;
        [SerializeField, Header("����㼶")]
        protected LayerMask m_groundLayer;

        [SerializeField, Header("�����㼶")]
        protected LayerMask m_climbLayer;

        [SerializeField, Header("ǽ�ܲ㼶")]
        protected LayerMask m_wallRunLayer;

        [SerializeField, Header("�������㼶")]
        protected LayerMask m_lockonLayer;

        [SerializeField, Header("������Χ")]
        protected float m_lockonRadius = 10f;

        [SerializeField, Header("�ƶ��ٶ�")]
        protected float m_moveSpeed = 1f;

        [SerializeField, Header("�ƶ��ٶ�ϵ��")]
        protected float m_moveMultiplier = 1f;

        [SerializeField, Header("��ת�ٶ�")]
        protected float m_rotateSpeed = 10f;

        [SerializeField, Header("��Ծ�߶�")]
        protected float m_jumpHeight = 2f;

        [SerializeField, Header("��Ծ����")]
        protected int m_jumpFrequency = 2;

        [SerializeField, Header("�������ٶ�")]
        protected float m_gravity = -9.8f;

        [SerializeField, Header("��������")]
        protected float m_airDamping = 2f;

        [SerializeField, Header("��������߶�")]
        protected float m_maxClimbHeight = 3f;
        #endregion
        /// <summary>
        /// ���ڵ�
        /// </summary>
        public Transform rootTransform { get; set; }
        /// <summary>
        /// ���ĵ�
        /// </summary>
        public Transform pelvisTransform { get; set; }
        /// <summary>
        /// ǰ���ٶ�
        /// </summary>
        public float forwardSpeed { get; set; }
        /// <summary>
        /// ��ֱ�ٶ�
        /// </summary>
        public float verticalSpeed { get; set; }
        /// <summary>
        /// �Ƿ��ŵ�
        /// </summary>
        public bool isGround { get; set; }
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool isFall { get; set; }
        /// <summary>
        /// ��ɫ������
        /// </summary>
        public CharacterController characterController { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public Animator animator { get; set; }
        /// <summary>
        /// �ƶ�ģʽ
        /// </summary>
        protected MoveType m_moveType = MoveType.RUN;
        /// <summary>
        /// ��Ϊģʽ
        /// </summary>
        protected MovementType m_movementType = MovementType.IDLE;
        /// <summary>
        /// �Ƿ�����״̬
        /// </summary>
        protected bool m_isGazing;
        /// <summary>
        /// Ŀ��Ƕ��뵱ǰ�Ƕȵļн�
        /// </summary>
        protected float m_targetDeg;
        /// <summary>
        /// �Ƿ�ס��Ծ
        /// </summary>
        protected bool m_holdJumpBtn;
        /// <summary>
        /// ��Ծ����
        /// </summary>
        protected int m_jumpCount;
        /// <summary>
        /// ��һ֡��ǰ����
        /// </summary>
        protected Vector3 m_lastForward;
        /// <summary>
        /// Ŀ�귽��
        /// </summary>
        protected Vector3 m_targetDirection;

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            rootTransform = characterController.gameObject.transform;
            pelvisTransform = transform.Find("Model/ClazyRunner/root/pelvis");
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void FixedUpdate()
        {
            
        }

        protected virtual void OnAnimatorMove()
        {
            
        }

        #region ƽ���ƶ�
        /// <summary>
        /// ��ת��Ŀ�귽��
        /// </summary>
        /// <param name="direction">Ŀ�귽��</param>
        /// <param name="rotationSpeed">��ת�ٶ�</param>
        public virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
        {
            direction.y = 0f;
            if (direction.normalized.magnitude == 0)
                direction = rootTransform.forward;

            var euler = rootTransform.rotation.eulerAngles.NormalizeAngle();
            var targetEuler = Quaternion.LookRotation(direction.normalized).eulerAngles.NormalizeAngle();
            euler.y = Mathf.LerpAngle(euler.y, targetEuler.y, rotationSpeed * Time.fixedDeltaTime);
            Quaternion newRotation = Quaternion.Euler(euler);
            rootTransform.rotation = newRotation;
        }

        public virtual void RotateToDirection(Vector3 direction)
        {
            RotateToDirection(direction, m_rotateSpeed);
        }

        public virtual void RotateByRootMotor()
        {
            rootTransform.rotation *= animator.deltaRotation;
        }


        public virtual void MoveToDirection(Vector3 direction)
        {
            direction.y = 0f;
            direction = direction.normalized * Mathf.Clamp(direction.magnitude, 0, 1f);
            //��һ֡���ƶ�λ��
            Vector3 targetPosition = rootTransform.position + direction * m_moveSpeed * m_moveMultiplier * Time.fixedDeltaTime;
            //��һ֡���ƶ��ٶ�
            Vector3 targetVelocity = (targetPosition - rootTransform.position) / Time.fixedDeltaTime;

            characterController.Move(targetVelocity);

        }

        public virtual void MoveByMotor()
        {
            characterController.Move(animator.deltaPosition);
        }

        #endregion

    }
}
