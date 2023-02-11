using System;
using MyUnitTools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runner.Input
{
    public class CharacterInputSystem : SingletonBase<CharacterInputSystem>
    {
        private PlayerInputAction _inputAction;


       public Vector2 playerMovementKey
       {
           get => _inputAction.Player.Movement.ReadValue<Vector2>();
       }
        
       public Vector2 playerCameraLook
       {
           get => _inputAction.Player.CameraLook.ReadValue<Vector2>();
       }
        
        public bool jumpKey
        {
            get => _inputAction.Player.JumpKey.triggered;
        }
        
        public bool runKey
        {
            get => _inputAction.Player.RunKey.phase == InputActionPhase.Performed;
        }
        
        public bool crouchKey
        {
            get => _inputAction.Player.CrouchKey.triggered;
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        private void Awake()
        {
            if (_inputAction == null)
                _inputAction = new PlayerInputAction();
        }

        private void OnEnable()
        {
            if(_inputAction != null)
                _inputAction.Enable();
                
        }

        private void OnDisable()
        {
            if(_inputAction != null)
                _inputAction.Disable();
        }
    }

}