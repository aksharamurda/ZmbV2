﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AksharaMurda
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerCamera Camera
        {
            get
            {
                //if (CameraOverride != null)
                //    return CameraOverride;
                //else
                //{
                if (CameraManager.Main != _cachedCameraOwner)
                {
                    _cachedCameraOwner = CameraManager.Main;

                    if (_cachedCameraOwner == null)
                        _cachedCamera = null;
                    else
                        _cachedCamera = _cachedCameraOwner.GetComponentInParent<PlayerCamera>();
                }

                return _cachedCamera;
                //}
            }
        }

        //[Tooltip("Camera to rotate around the player. If set to none it is taken from the main camera.")]
        //public ThirdPersonCamera CameraOverride;

        

        [Tooltip("Multiplier for horizontal camera rotation.")]
        [Range(0, 10)]
        public float HorizontalRotateSpeed = 0.9f;
        [Range(0, 10)]
        public float MobileHorizontalRotateSpeed = 0.9f;
        [Tooltip("Multiplier for vertical camera rotation.")]
        [Range(0, 10)]
        public float VerticalRotateSpeed = 1.0f;
        [Range(0, 10)]
        public float MobileVerticalRotateSpeed = 1.0f;
        [Tooltip("Is camera responding to mouse movement when the mouse cursor is unlocked.")]
        public bool RotateWhenUnlocked = false;

        [Tooltip("Maximum time in seconds to wait for a second tap to active rolling.")]
        public float DoubleTapDelay = 0.3f;

        private PlayerMotor _motor;
        private PlayerController _controller;

        private Camera _cachedCameraOwner;
        private PlayerCamera _cachedCamera;

        private float _timeW;
        private float _timeA;
        private float _timeS;
        private float _timeD;

        [HideInInspector]
        public float pointer_x;
        [HideInInspector]
        public float pointer_y;

        private float HorRotateSpeed = 0f;
        private float VerRotateSpeed = 0f;

        public bool useClampPlayer;
        [Range(0, -30)]
        public float minVerticalValue = -30;
        [Range(0, 30)]
        public float maxVerticalValue = 30;
        [Range(0, -60)]
        public float minHorizontalValue = -30;
        [Range(0, 60)]
        public float maxHorizontalValue = 30;
        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _motor = GetComponent<PlayerMotor>();
        }

        void Update()
        {

            if (!GlobalManager.instance.useMobileConsole)
            {
                pointer_x = Input.GetAxis("Mouse X");
                pointer_y = Input.GetAxis("Mouse Y");
                HorRotateSpeed = HorizontalRotateSpeed;
                VerRotateSpeed = VerticalRotateSpeed;
            }
            else
            {
                HorRotateSpeed = MobileHorizontalRotateSpeed;
                VerRotateSpeed = MobileVerticalRotateSpeed;
            }

            UpdateCamera();
            UpdateTarget();
            UpdateMovement();
            UpdateWeapons();
            UpdateReload();
            UpdateRolling();
            UpdateFireAndZoom();
            UpdateGrenade();
            UpdateCrouching();
            UpdateClimbing();
            UpdateCover();
            UpdateJumping();
        }

        protected virtual void UpdateMovement()
        {
            var movement = new PlayerMovement();

            var direction = Input.GetAxis("Horizontal") * Vector3.right +
                            Input.GetAxis("Vertical") * Vector3.forward;

            var lookAngle = Util.AngleOfVector(_controller.LookTargetInput - _motor.transform.position);

            if (direction.magnitude > float.Epsilon)
                movement.Direction = Quaternion.Euler(0, lookAngle, 0) * direction.normalized;

            if (_controller.ZoomInput)
                movement.Magnitude = 0.5f;
            else
            {
                if (_motor.Gun != null)
                {
                    if (Input.GetButton("Run") && !_motor.IsCrouching)
                        movement.Magnitude = 2.0f;
                    else
                        movement.Magnitude = 1.0f;
                }
                else
                {
                    if (Input.GetButton("Run"))
                        movement.Magnitude = 1.0f;
                    else
                        movement.Magnitude = 0.5f;
                }
            }

            _controller.MovementInput = movement;
        }

        protected virtual void UpdateClimbing()
        {
            if (Input.GetButtonDown("Climb"))
                if (Input.GetAxis("Vertical") > 0.1f)
                    if (_motor.IsInCover && _motor.CanClimbOrVault)
                        _motor.InputClimbOrVault();
        }

        protected virtual void UpdateCover()
        {
            if (Input.GetButtonDown("TakeCover"))
                if (!_controller.AutoTakeCover && !_motor.IsInCover && _motor.PotentialCover != null)
                    _motor.InputTakeCover();
        }

        protected virtual void UpdateJumping()
        {
            if (Input.GetButtonDown("Jump"))
                _motor.InputJump();
        }

        protected virtual void UpdateCrouching()
        {
            if (!_motor.IsSprinting && Input.GetButton("Crouch"))
                _motor.InputCrouch();
        }

        protected virtual void UpdateGrenade()
        {
            if (_motor.HasGrenadeInHand)
            {
                if (Input.GetButtonDown("Fire1"))
                    _controller.InputThrowGrenade();

                if (Input.GetButtonDown("Fire2"))
                    _motor.InputCancelGrenade();

                if (!_motor.IsReadyToThrowGrenade)
                    if (Input.GetButton("Grenade"))
                        _motor.InputTakeGrenade();
            }
            else
            {
                if (Input.GetButton("Grenade"))
                    _motor.InputTakeGrenade();
            }
        }

        protected virtual void UpdateFireAndZoom()
        {
            if (!GlobalManager.instance.useMobileConsole)
            {
                if (Input.GetButtonDown("Fire1")) _controller.FireInput = true;
                if (Input.GetButtonUp("Fire1")) _controller.FireInput = false;

                if (Input.GetButtonDown("Fire2")) _controller.ZoomInput = true;
                if (Input.GetButtonUp("Fire2")) _controller.ZoomInput = false;

            }
        }

        public void OnMobileFire(bool input)
        {
            _controller.FireInput = input;
            _controller.ZoomInput = input;
        }

        public void OnMobileReload()
        {
            if (!_motor.HasGrenadeInHand)
                _motor.InputReload();
        }

        public void OnMobileSwitch()
        {
            if (_motor.CurrentWeapon == _motor.Weapons.Length)
                _motor.InputWeapon(1);
            else
                _motor.InputWeapon(_motor.CurrentWeapon + 1);
        }

        protected virtual void UpdateRolling()
        {
            if (_timeW > 0) _timeW -= Time.deltaTime;
            if (_timeA > 0) _timeA -= Time.deltaTime;
            if (_timeS > 0) _timeS -= Time.deltaTime;
            if (_timeD > 0) _timeD -= Time.deltaTime;

            if (Input.GetButtonDown("RollForward"))
            {
                if (_timeW > float.Epsilon)
                {
                    if (_motor.IsInCover && _motor.CanClimbOrVault)
                        _motor.InputClimbOrVault();
                    else
                        _motor.InputRoll(_motor.LookAngle);
                }
                else
                    _timeW = DoubleTapDelay;
            }

            if (Input.GetButtonDown("RollLeft"))
            {
                if (_timeA > float.Epsilon)
                    _motor.InputRoll(_motor.LookAngle - 90);
                else
                    _timeA = DoubleTapDelay;
            }

            if (Input.GetButtonDown("RollBackward"))
            {
                if (_timeS > float.Epsilon)
                    _motor.InputRoll(_motor.LookAngle + 180);
                else
                    _timeS = DoubleTapDelay;
            }

            if (Input.GetButtonDown("RollRight"))
            {
                if (_timeD > float.Epsilon)
                    _motor.InputRoll(_motor.LookAngle + 90);
                else
                    _timeD = DoubleTapDelay;
            }
        }

        protected virtual void UpdateWeapons()
        {
            if (Input.GetKey(KeyCode.Alpha1)) { _motor.InputCancelGrenade(); _motor.InputWeapon(0); }
            if (Input.GetKey(KeyCode.Alpha2)) { _motor.InputCancelGrenade(); _motor.InputWeapon(1); }
            if (Input.GetKey(KeyCode.Alpha3)) { _motor.InputCancelGrenade(); _motor.InputWeapon(2); }
            if (Input.GetKey(KeyCode.Alpha4)) { _motor.InputCancelGrenade(); _motor.InputWeapon(3); }
            if (Input.GetKey(KeyCode.Alpha5)) { _motor.InputCancelGrenade(); _motor.InputWeapon(4); }
            if (Input.GetKey(KeyCode.Alpha6)) { _motor.InputCancelGrenade(); _motor.InputWeapon(5); }
            if (Input.GetKey(KeyCode.Alpha7)) { _motor.InputCancelGrenade(); _motor.InputWeapon(6); }
            if (Input.GetKey(KeyCode.Alpha8)) { _motor.InputCancelGrenade(); _motor.InputWeapon(7); }
            if (Input.GetKey(KeyCode.Alpha9)) { _motor.InputCancelGrenade(); _motor.InputWeapon(8); }
            if (Input.GetKey(KeyCode.Alpha0)) { _motor.InputCancelGrenade(); _motor.InputWeapon(9); }

            if (!GlobalManager.instance.useMobileConsole)
            {
                if (Input.mouseScrollDelta.y < 0)
                {
                    if (_motor.CurrentWeapon == 0)
                        _motor.InputWeapon(_motor.Weapons.Length);
                    else
                        _motor.InputWeapon(_motor.CurrentWeapon - 1);
                }
                else if (Input.mouseScrollDelta.y > 0)
                {
                    if (_motor.CurrentWeapon == _motor.Weapons.Length)
                        _motor.InputWeapon(0);
                    else
                        _motor.InputWeapon(_motor.CurrentWeapon + 1);
                }
            }

        }

        protected virtual void UpdateReload()
        {
            if (!_motor.HasGrenadeInHand)
                if (Input.GetButton("Reload"))
                    _motor.InputReload();
        }

        protected virtual void UpdateTarget()
        {
            if (_controller == null)
                return;



            var camera = Camera;
            if (camera == null) return;

            var lookPosition = camera.transform.position + camera.transform.forward * 1000;

            _controller.LookTargetInput =  lookPosition;
            _controller.GrenadeHorizontalAngleInput = Util.AngleOfVector(camera.transform.forward);
            _controller.GrenadeVerticalAngleInput = Mathf.Asin(camera.transform.forward.y) * 180f / Mathf.PI;

            var closestHit = Util.GetClosestHit(camera.transform.position, lookPosition, Vector3.Distance(camera.transform.position, _motor.Top), gameObject);

            if (_motor.TurnSettings.IsAimingPrecisely)
                closestHit += transform.forward;

            _controller.FireTargetInput = closestHit;
        }

        protected virtual void UpdateCamera()
        {
            var camera = Camera;
            if (camera == null) return;

            //if (IsPointerOverGameObject())
            //    return;

            var scale = 1.0f;

            if (_controller.IsZooming && _motor != null && _motor.Gun != null)
                scale = 1.0f - _motor.Gun.Zoom / camera.StateFOV;



            camera.Horizontal = Mathf.LerpAngle(camera.Horizontal, camera.Horizontal + pointer_x * HorRotateSpeed * Time.timeScale * scale, 1.0f);
            camera.Vertical = Mathf.LerpAngle(camera.Vertical, camera.Vertical - pointer_y * VerRotateSpeed * Time.timeScale * scale, 1.0f);

            if (useClampPlayer)
            {
                camera.Horizontal = Mathf.Clamp(camera.Horizontal, minHorizontalValue, maxHorizontalValue);
                camera.Vertical = Mathf.Clamp(camera.Vertical, minVerticalValue, maxVerticalValue);
            }

            camera.UpdatePosition();

        }

    }
}
