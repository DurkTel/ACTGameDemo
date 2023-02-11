using UnityEngine;
using Runner.Camera;
using Runner.Input;

namespace TPP.Move
{
    public class TPP_Character_PlayerMovement : TPP_CharacterMovementBase
    {
        //引用
        private Transform tpCamera;
        private TP_CameraController _cameraController;

        [SerializeField, Header("相机锁定点")] private Transform defaultCameraLookAtTarget;
        [SerializeField] private Transform crouchCameraLookAtTarget;

        //角色移动相关
        private float characterRotation = 0f;
        private float characterRationVelocity = 0f;
        [SerializeField,Header("角色旋转平滑速度"),Range(0.1f,1.0f)]private float characterRotationLerpTime;
        [SerializeField, Header("角色移动方向平滑度"),Range(1f,10.0f)]
        private float characterMoveDirectionLerpTime;

        private float deltaAngle;
        private Vector3 lastForward;
        
        protected override void Awake()
        {
            base.Awake();
            tpCamera = UnityEngine.Camera.main.transform.root;
            _cameraController = tpCamera.GetComponent<TP_CameraController>();
            
        }

        protected override void Start()
        {
            base.Start();
            _cameraController.SetCameraLookAtTarget(defaultCameraLookAtTarget);
            lastForward = transform.forward;

        }

        protected override void Update()
        {
            base.Update();
            
            UpdateBasicMotionAnimtion();
            UpdateCharacterAngleSpeed();
        }

        #region 移动

        protected override void CharacterBasicMovement()
        {
            if (characterIsOnGround )
            {
                if (CharacterInputSystem.Instance.playerMovementKey != Vector2.zero && characterAnimator.CheckAnimationTag("Motion"))
                {
                    
                    //处理玩家的旋转方向
                    characterRotation =
                        Mathf.Atan2(CharacterInputSystem.Instance.playerMovementKey.x,
                            CharacterInputSystem.Instance.playerMovementKey.y) * Mathf.Rad2Deg + tpCamera.eulerAngles.y;

                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y,
                        characterRotation,
                        ref characterRationVelocity, characterRotationLerpTime);
                    
                    //取角色移动方向,乘以Vector3.forward将四元数转成Vector3
                    var direction = Quaternion.Euler(0, characterRotation, 0) * Vector3.forward;
                    direction = direction.normalized;
                    movementNormalDirection = Vector3.Slerp(movementNormalDirection,
                        ResetMoveDirectionOnSlop(direction),
                        this.MyLerp(characterMoveDirectionLerpTime));

                    deltaAngle = -GetDeltaAngle(direction);
                    deltaAngle *= 0.002f;

                }
                else
                {
                    movementNormalDirection = Vector3.zero;
                }
            }

            if (!CharacterMoveDirectionHasObject(movementNormalDirection))
            {
                characterController.Move(characterAnimator.GetFloat(moveSpeedID) * Time.deltaTime * movementNormalDirection.normalized);
                Debug.DrawRay(transform.position,movementNormalDirection * 2f,Color.black);
            }
            
        }

        private bool CanRun()
        {
            if (characterIsOnGround && CharacterInputSystem.Instance.playerMovementKey.sqrMagnitude>0f)
            {
                if (Vector3.Dot(transform.forward,movementNormalDirection.normalized) > 0.75f)
                    return true;
            }

            return false;
        }

        #endregion

        #region 更新动画

       
        private void UpdateBasicMotionAnimtion()
        {

            if (characterAnimator.CheckAnimationTag("Motion"))
            {
                if (characterAnimator.GetFloat(moveID) < 1.1f)
                {
                    characterAnimator.SetFloat(moveID,Mathf.Lerp(characterAnimator.GetFloat(moveID),
                        CharacterInputSystem.Instance.playerMovementKey.sqrMagnitude,
                        this.MyLerp(3f)));
                }

                if (characterAnimator.GetFloat(moveID) > 0.5f && CharacterInputSystem.Instance.playerMovementKey!=Vector2.zero)
                {
                    if (CharacterInputSystem.Instance.runKey)
                    {
                        characterAnimator.SetFloat(moveID,2f,0.25f,Time.deltaTime);
                    }
                    else
                    {
                        characterAnimator.SetFloat(moveID,Mathf.Lerp(characterAnimator.GetFloat(moveID),
                            CharacterInputSystem.Instance.playerMovementKey.sqrMagnitude,
                            this.MyLerp(3f)));
                    }
                }
                else
                {
                    characterAnimator.SetFloat(moveID,Mathf.Lerp(characterAnimator.GetFloat(moveID),
                        CharacterInputSystem.Instance.playerMovementKey.sqrMagnitude,
                        this.MyLerp(3f)));
                }

            }
        }
        
        #endregion

        #region 计算角速度

        private float GetLocalForwardAangle(Vector3 dir)
        {
            var localForward = transform.InverseTransformDirection(dir);

            return Mathf.Atan2(localForward.x, localForward.z) * Mathf.Rad2Deg;
        }

        private float GetDeltaAngle(Vector3 dir)
        {
            return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        }

        private void UpdateCharacterAngleSpeed()
        {
            float angleSpeed = -GetLocalForwardAangle(lastForward) - deltaAngle;
            deltaAngle = 0f;
            lastForward = transform.forward;
            angleSpeed *= 0.002f;
            angleSpeed = Mathf.Clamp(angleSpeed / Time.deltaTime, -1f, 1f);//角速度公示 角速度= 角度/t;
            //Debug.Log(angleSpeed);

            characterAnimator.SetFloat(trunID,angleSpeed);
        }
        
        #endregion
        
        
    }
}
