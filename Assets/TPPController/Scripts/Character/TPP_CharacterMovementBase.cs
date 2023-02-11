using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPP.Move
{
    public abstract class TPP_CharacterMovementBase : MonoBehaviour
    {
         //引用
        protected CharacterController characterController;
        protected Animator characterAnimator;
        


        //移动方向
        protected Vector3 movementNormalDirection;//正常角色出于地面的移动方向
        protected Vector3 movementVerticalDirection;//用于处理角色垂直移动量
        
        //角色速度量
        protected float characterGravity = -20f;//角色重力
        protected float characterVerticalSpeed;//角色垂直移动速度(角色跳跃高度，角色跌落速度)
        private float fallOutTime = 0.15f;
        private float fallOutDeltaTime;
        private float characterMaxVerticalSpeed = 53f;

        

        [SerializeField,Header("地面检测")] private float heightOffset;
        [SerializeField] private float detectionRadius;
        [SerializeField] private float detectionObsRang;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private LayerMask whatIsObs;
        [SerializeField] protected bool characterIsOnGround;
        private bool isUseGravity = true;



        private float originHeight;
        private Vector3 originCenter;
        private float originRadius;
        [SerializeField, Header("角色胶囊")] private float crouchHeight;
        [SerializeField] private Vector3 crouchCenter;
        [SerializeField] private float crouchRadius;

        
        //Streing To Hash
        protected readonly int moveSpeedID = Animator.StringToHash("MoveSpeed");
        protected readonly int moveID = Animator.StringToHash("Move");
        protected readonly int trunID = Animator.StringToHash("Trun");
        protected readonly int deltaAngleID = Animator.StringToHash("DeltaAngle");
        

        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
            characterAnimator = GetComponentInChildren<Animator>();
           
        }

        protected virtual void Start()
        {
            fallOutDeltaTime = fallOutTime;
            
            //初始化胶囊尺寸
            originCenter = characterController.center;
            originHeight = characterController.height;
            originRadius = characterController.radius;

        }

        protected virtual void Update()
        {
            CalculateCharacterGravity();
            UpdateCharacterGraivty();
            CheckOnGround();
            CharacterBasicMovement();
            
        }
        //=====================================================================================================

        #region 角色重力

        private void CalculateCharacterGravity()
        {
            if (characterIsOnGround)
            {
                fallOutDeltaTime = fallOutTime;
                if (characterVerticalSpeed < 0)
                    characterVerticalSpeed = -2f;
            }
            else
            {
                if (fallOutDeltaTime >= 0.0f) fallOutDeltaTime -= Time.deltaTime;
            }

            if (characterVerticalSpeed < characterMaxVerticalSpeed)
                characterVerticalSpeed += characterGravity * Time.deltaTime;
        }

        private void UpdateCharacterGraivty()
        {
            if (isUseGravity)
                movementVerticalDirection.Set(0f,characterVerticalSpeed,0f);
            else
                movementVerticalDirection = Vector3.zero;
            
            characterController.Move( Time.deltaTime * movementVerticalDirection);
        }

        #endregion

        #region 角色地面检测

        private void CheckOnGround()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - heightOffset, transform.position.z);
            characterIsOnGround = Physics.CheckSphere(spherePosition, detectionRadius, whatIsGround, QueryTriggerInteraction.Ignore);
        }

        private void OnDrawGizmosSelected()
        {
            
            if (characterIsOnGround) 
                Gizmos.color = Color.green;
            else 
                Gizmos.color = Color.red;

            Vector3 position = Vector3.zero;
            
            position.Set(transform.position.x, transform.position.y - heightOffset,
                transform.position.z);

            Gizmos.DrawWireSphere(position, detectionRadius);
            
        }

        #endregion

        #region 坡度检测

        protected Vector3 ResetMoveDirectionOnSlop(Vector3 dir)
        {
            if (Physics.Raycast(transform.position, -Vector3.up, out var hit, 10f))
            {
                float newAnle = Vector3.Dot(Vector3.up, hit.normal);
                
                if (newAnle != 0 && characterVerticalSpeed <= 0)
                {
                    return Vector3.ProjectOnPlane(dir, hit.normal);
                }
            }
            return dir;
        }

        #endregion
        
        #region 角色移动

        protected bool CharacterMoveDirectionHasObject(Vector3 dir)
        {
            return Physics.Raycast(transform.position + transform.up * 1.5f, dir,.6f,whatIsObs);
        }
        
        
        /// <summary>
        /// 角色的基本移动
        /// </summary>
        protected virtual void CharacterBasicMovement()
        {
            
        }
        
        /// <summary>
        /// 角色移动接口,不处理Y轴移动
        /// </summary>
        /// <param name="moveDirection">移动方向</param>
        /// <param name="moveSpeed">移动速度</param>
        public void CharacterBasicMovementInterface(Vector3 moveDirection,float moveSpeed)
        {
            if(moveDirection.y>0 || moveDirection.y<0) return;
            
            if (!CharacterMoveDirectionHasObject(moveDirection*characterAnimator.GetFloat(moveSpeedID)))
            {
                movementNormalDirection = ResetMoveDirectionOnSlop(moveDirection).normalized;

                characterController.Move(moveSpeed * Time.deltaTime * movementNormalDirection);
            }
        }

        public Vector3 GetCharacterVelocity() => characterController.velocity;
        
        /// <summary>
        /// 设置胶囊大小为下蹲状态大小
        /// </summary>
        public void SetCharacterControllerCrouchSize()
        {
            characterController.height = crouchHeight;
            characterController.center = crouchCenter;
            characterController.radius = crouchRadius;
        }

        /// <summary>
        /// 设置胶囊大小为初始状态大小
        /// </summary>
        public void ResetCharacterControllerSize()
        {
            characterController.height = originHeight;
            characterController.center = originCenter;
            characterController.radius = originRadius;
        }

        #endregion
    }
}
