using System.Collections;
using System.Collections.Generic;
using UI_InputSystem.Base;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.VGS
{
    /// <summary>
    /// First person charactor controller
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Field

        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _mouseSpeed = 100f;
        [SerializeField] private float _gravity = -9.81f;
        public static float speed = 4f;
        public CharacterController _controller;
        private Camera _mainCamera;

        private float _xRotation = 0f;

        private float _mouseX, _mouseY;
        private float _horizontal, _vertical;

        private Vector3 _velocity;

        public bool IsAllowRotate { get; set; } = false;
        public bool IsAllowMove { get; set; } = false;

        public static bool isDrag = false;
        public static bool isEdit = false;
        public static bool isShowMenu = false;
        public bool canFly = false, PopupShowing = false;

        [SerializeField] private Texture2D _defaultCursor;
        [SerializeField] private Texture2D _rotateCursor;
        [SerializeField] private Texture2D _handCursor;
        [SerializeField] private Texture2D _productCursor;

        private float XAxisS
        {
            get
            {
                if (WebGLAdapter.IsMobileDevice)
                    return UIInputSystem.ME.GetAxisHorizontal(JoyStickAction.CameraLook);
                return Input.GetAxis("Mouse X");
            }
        }
        private float YAxis
        {
            get
            {
                if (WebGLAdapter.IsMobileDevice)
                    return UIInputSystem.ME.GetAxisVertical(JoyStickAction.CameraLook);
                return Input.GetAxis("Mouse Y");
            }
        }
        private float horizontalAxis
        {
            get
            {
                if (WebGLAdapter.IsMobileDevice)
                    return UIInputSystem.ME.GetAxisHorizontal(JoyStickAction.Movement);
                return Input.GetAxis("Horizontal");
            }
        }
        private float verticalAxis
        {
            get
            {
                if (WebGLAdapter.IsMobileDevice)
                    return UIInputSystem.ME.GetAxisVertical(JoyStickAction.Movement);
                return Input.GetAxis("Vertical");
            }
        }

        #endregion


        private void Awake()
        {
            _controller = gameObject.GetComponent<CharacterController>();
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            if (WebGLAdapter.IsMobileDevice)
                IsAllowRotate = true;
        }

        private void Update()
        {
            _moveSpeed = speed;
            if (Input.GetMouseButton(1))
            {
                IsAllowRotate = true;

                if (isDrag)
                {
                    SetHandCursor();
                }
                else if (isShowMenu)
                    SetDefaultCursor();
                else
                {
                    SetRotateCursor();
                }
            }
            else
            {
                if (!isEdit)
                {
                    SetDefaultCursor();
                }
                else
                {
                    SetProductCursor();
                }
                if (Input.GetMouseButtonUp(1))
                {
                    IsAllowRotate = false;
                    SetDefaultCursor();
                    isDrag = false;
                }
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                canFly = !canFly;
                if (canFly)
                {
                    _gravity = 0;
                }
                else
                {
                    _gravity = -9.81f;
                }
            }
            if (WebGLAdapter.IsMobileDevice || (!isDrag && IsAllowRotate))
            {
                UpdateCamera();
            }
            if (canFly)
            {
                UpdateFlyMovement();
            }
            if (IsAllowMove)
                UpdateMovement();
        }
        //////////////////////////////////////
        /// <summary>
        /// Update Camera Rotation with Mouse Input
        /// </summary>
        /// 

        private void UpdateCamera()
        {
            //Task 740 - dont need lock Rotate
            //if (!PopupShowing && !isEdit && !isShowMenu)
            //if (!isEdit)
            //{
            _mouseX = XAxisS * _mouseSpeed * Time.deltaTime;
            _mouseY = YAxis * _mouseSpeed * Time.deltaTime;

            _xRotation -= _mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            _mainCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * _mouseX);
            //}
            //DebugExtension.LogError(XAxisS + "    " + YAxis);
        }

        /// <summary>
        /// Update Character Movement with Key Input 
        /// </summary>
        private void UpdateMovement()
        {
            //Task 740 - dont need lock Move
            //if (!PopupShowing && !isEdit && !isShowMenu)
            //if (!isEdit)
            //{
            if (WebGLAdapter.IsMobileDevice)
            {
                Move();
            }
            else
            {

                Vector3 move;
                if (Input.GetMouseButton(2))
                {
                    // mouse wheeel Scroll
                    _horizontal = XAxisS * _mouseSpeed * 0.7f * Time.deltaTime;
                    _vertical = YAxis * _mouseSpeed * 0.7f * Time.deltaTime;
                    move = transform.right * _horizontal + transform.forward * _vertical;
                    _controller.Move(move * _moveSpeed * Time.deltaTime);
                }
                else
                {
                    if (Input.mouseScrollDelta.y == 0)
                    {
                        Move();
                    }
                    else
                    {
                        List<RaycastResult> results = null;
                        bool isOverUI = DragObjectManagerV2.IsPointerOverCanvas(list =>
                        {
                            results = list;
                        });

                        if (!isOverUI && !WebGLAdapter.IsOverlayMenuVideoChat)
                        {
                            Move();
                        }
                        else
                        {
                            if (MenuTabControllerV2.Instance.TabTutorial.activeSelf)
                            {
                                foreach (var ray in results)
                                {
                                    MenuTabTutorial tutorial = ray.gameObject.GetComponentInParent<MenuTabTutorial>();
                                    if (tutorial)
                                    {
                                        Move();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //}
            if (!canFly)
            {
                if (_controller.isGrounded && _velocity.y < 0)
                {
                    _velocity.y = -2f;
                }
                else
                {
                    _velocity.y += _gravity * Time.deltaTime;
                    _controller.Move(_velocity * Time.deltaTime);
                }
            }
        }

        void Move()
        {
            _horizontal = horizontalAxis;
            _vertical = verticalAxis + Input.mouseScrollDelta.y * _moveSpeed;
            Vector3 move = transform.right * _horizontal + transform.forward * _vertical;
            _controller.Move(move * _moveSpeed * Time.deltaTime);
        }

        private void UpdateFlyMovement()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                _velocity.y = -2;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                _velocity.y = 2;
            }
            else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
            {
                _velocity.y = 0;
            }

            _controller.Move(_velocity * Time.deltaTime);
        }

        public void SetRotateCursor()
        {
            Cursor.SetCursor(_rotateCursor, Vector2.zero, CursorMode.Auto);
        }

        public void SetDefaultCursor()
        {
            Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
        }
        public void SetHandCursor()
        {
            Cursor.SetCursor(_handCursor, Vector2.zero, CursorMode.Auto);
        }
        public void SetProductCursor()
        {
            Cursor.SetCursor(_productCursor, new Vector2(0, 20), CursorMode.Auto);
        }

        public void ActiveCharacter(bool isActive)
        {
            _controller.enabled = isActive;
        }
    }

}
