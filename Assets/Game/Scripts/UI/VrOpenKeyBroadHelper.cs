using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client
{
    public class VrOpenKeyBroadHelper : MonoBehaviour
    {
        private static VrInputKeyBoard vrInputKeyBoardPrefab;
        private static VrInputKeyBoard vrInputKeyBoardCurrent;

        private static VrInputKeyBoard VrInputKeyBoardPrefab => vrInputKeyBoardPrefab
            ? vrInputKeyBoardPrefab
            : vrInputKeyBoardPrefab = Resources.Load<VrInputKeyBoard>("UI/Keyboard/VrKeyboard");

        private InputField inputCurrent;

        [SerializeField] private Vector3 targetLocalScale = Vector3.one;
        [SerializeField] bool isUseXLocalPositions = false;
        [SerializeField] private float targetXLocalPosition = 0;
        [SerializeField] bool isUseYLocalPosition = false;
        [SerializeField] private float targetYLocalPosition;
        [SerializeField] private bool isNormalKeyboard = true;
        public int orderLayer = 5;

        private static VrInputKeyBoard VrInputKeyBoardCurrent => vrInputKeyBoardCurrent
            ? vrInputKeyBoardCurrent
            : vrInputKeyBoardCurrent = Instantiate(VrInputKeyBoardPrefab);

        System.IDisposable openKeyboardDisposable;

        private void Awake()
        {
            inputCurrent = GetComponent<InputField>();
        }

        private void Start()
        {
            RegisterOpenKeyboard(true);
        }

        public void RegisterOpenKeyboard(bool value)
        {
            if (value)
            {
                openKeyboardDisposable?.Dispose();
                openKeyboardDisposable = inputCurrent.OnPointerDownAsObservable()
                .Subscribe(_ => OpenKeyboard(inputCurrent.transform));
            }
            else
            {
                openKeyboardDisposable?.Dispose();
                openKeyboardDisposable = null;
            }
        }

        private void OnDisable()
        {
            Destroy(VrInputKeyBoardCurrent.gameObject);
        }

        private void OpenKeyboard(Transform parent)
        {
            VrInputKeyBoardCurrent.HideKeyBoard();
            VrInputKeyBoardCurrent.OpenKeyBoard(parent, isNormalKeyboard);
            Setup(VrInputKeyBoardCurrent);
            VrInputKeyBoardCurrent.transform.localScale = targetLocalScale;
            VrInputKeyBoardCurrent.GetComponent<Canvas>().sortingOrder = orderLayer;

            if (!isUseYLocalPosition)
                targetYLocalPosition = VrInputKeyBoardCurrent.transform.localPosition.y;

            if (!isUseXLocalPositions)
            {
                Canvas canvas = GetComponentInParent<Canvas>();
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    VrInputKeyBoardCurrent.transform.position = inputCurrent.transform.position;
                }
                else
                {
                    if (canvas && canvas.name.IndexOf("PasswordForEditDialog") != -1)
                    {
                        // if input is password --> nothing
                    }
                    else
                    {
                        targetXLocalPosition = 0;
                        if (Camera.main)
                        {
                            Vector3 pos = Camera.main.WorldToScreenPoint(VrInputKeyBoardCurrent.right.transform.position);
                            DebugExtension.Log(pos);
                            DebugExtension.Log("width = " + Screen.width);
                            if (pos.x > Screen.width)
                            {
                                targetXLocalPosition = -(pos.x - Screen.width);
                            }
                            else
                            {
                                pos = Camera.main.WorldToScreenPoint(VrInputKeyBoardCurrent.left.transform.position);
                                DebugExtension.Log(pos);
                                //DebugExtension.Log("targetXLocalPosition = " + );
                                if (pos.x < 0)
                                {
                                    targetXLocalPosition = -pos.x;
                                }
                            }
                        }
                    }
                }
                //DebugExtension.LogError(targetXLocalPosition);
                //DebugExtension.LogError(targetYLocalPosition);
            }

            VrInputKeyBoardCurrent.transform.localPosition = new Vector3(targetXLocalPosition, targetYLocalPosition, VrInputKeyBoardCurrent.transform.localPosition.z - 5);

            if (GetComponent<KeyboardPositionCustom>()) GetComponent<KeyboardPositionCustom>().SetPosition(VrInputKeyBoardCurrent);
        }

        private void Setup(VrInputKeyBoard vrInputKeyBoard)
        {
            vrInputKeyBoard.OnClickKey += VrInputKeyBoardOnOnClickKey;
            vrInputKeyBoard.OnClickEnter += VrInputKeyBoardOnOnClickEnter;
            vrInputKeyBoard.OnClickBackspace += VrInputKeyBoardOnOnClickBackspace;
        }

        private void VrInputKeyBoardOnOnClickBackspace()
        {
            if (inputCurrent.text.Length > 0)
            {
                inputCurrent.text = inputCurrent.text.Substring(0, inputCurrent.text.Length - 1);
                //DebugExtension.Log(inputCurrent.text);
                //DebugExtension.Log(inputCurrent.textComponent.text);
                //if(inputCurrent.text.Equals(inputCurrent.textComponent.text))
                //    inputCurrent.text = inputCurrent.text.Substring(0, inputCurrent.text.Length - 1);
                //else
                //{
                //    var not_show_characters = inputCurrent.text.Length - inputCurrent.textComponent.text.Length;
                //    inputCurrent.text = inputCurrent.text.Substring(0, inputCurrent.text.Length - not_show_characters - 1);
                //}
            }
        }

        private void VrInputKeyBoardOnOnClickEnter()
        {
            DebugExtension.Log("You've typed [" + inputCurrent.text + "]");
            inputCurrent.onEndEdit.Invoke(inputCurrent.text);
            //inputCurrent.text = "";
        }

        private void VrInputKeyBoardOnOnClickKey(string character)
        {
            inputCurrent.text += character;
        }
    }
}