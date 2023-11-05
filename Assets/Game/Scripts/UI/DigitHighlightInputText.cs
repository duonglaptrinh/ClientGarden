using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client
{

    [RequireComponent(typeof(InputField))]
    public class DigitHighlightInputText : MonoBehaviour
    {
        public string digit = "|";
        public float interval = 0.5f;

        protected InputField inputField;
        float countTime;
        protected Text textDigit;
        GameObject mask;
        bool showDigit;

        void Start()
        {
            inputField = GetComponent<InputField>();
            mask = Instantiate(Resources.Load<GameObject>("UI/Mask"), inputField.transform);
            mask.GetComponent<RectTransform>().sizeDelta = GetSourceText().GetComponent<RectTransform>().sizeDelta;
            mask.transform.position = GetSourceText().transform.position;

        }

        void Update()
        {
            bool editing = inputField.isFocused || GetComponentInChildren<VrInputKeyBoard>();

            if (!editing)
            {
                if (textDigit) Destroy(textDigit.gameObject);
                GetSourceText().enabled = true;
                return;
            }

            //if (editing && inputField.text.Length > 0)
            if (editing)
            {
                Text sourceText = GetSourceText();
                if (mask != null && textDigit == null)
                {
                    textDigit = Instantiate(sourceText, mask.transform);
                    textDigit.supportRichText = true;
                    textDigit.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                    textDigit.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                }
                textDigit.enabled = true;
            }
            else
            {
                if (textDigit != null)
                    textDigit.enabled = false;
            }

            SetLastText();

            //if (editing && inputField.text.Length > 0 && countTime <= Time.time)
            if (editing && countTime <= Time.time)
            {
                countTime = Time.time + interval;
                showDigit = !showDigit;
            }
        }

        protected virtual Text GetSourceText()
        {
            return inputField.textComponent;
        }

        public const string COLOR_CODE = "#ff000000";
        protected virtual string GetDisplayText()
        {
            return $"{inputField.text}{digit}";
        }

        protected virtual void SetLastText()
        {
            if (textDigit == null)
                return;

            //ConvertText(inputField);
            textDigit.text = inputField.text;

            if (inputField.inputType == InputField.InputType.Password)
                textDigit.text = ConvertSamePasswork(textDigit.text);
            else
            {
               ConvertText(textDigit);
            }
            textDigit.text = showDigit ? $"{textDigit.text}{digit}" : $"{textDigit.text} ";

            if (textDigit.text.Length - 1 > GetSourceText().text.Length)
            {
                textDigit.alignment = GetSourceText().alignment == TextAnchor.MiddleLeft ? TextAnchor.MiddleRight : TextAnchor.LowerLeft;
            }
            else
            {
                textDigit.alignment = GetSourceText().alignment;
            }

            GetSourceText().enabled = false;
        }

        protected virtual void ConvertText(Text text)
        {
        }
        protected virtual void ConvertText(InputField input)
        {
        }

        string ConvertSamePasswork(string s)
        {
            Regex pattern = new Regex("\\S");
            return pattern.Replace(s, "*");
        }

        // protected virtual Vector3 GetDigitPosition()
        // {
        //     Text textCom = inputField.textComponent;
        //     RectTransform rectTrans = textCom.GetComponent<RectTransform>();
        //     float x = textCom.preferredWidth / 2f;
        //     float y = textCom.transform.localPosition.y;

        //     if(textCom.alignment.ToString().Contains("Left"))
        //         x = rectTrans.rect.xMin + textCom.preferredWidth;
        //     else if(textCom.alignment.ToString().Contains("Right"))
        //         x = rectTrans.rect.xMax - textCom.preferredWidth;

        //     if(textCom.alignment.ToString().Contains("Upper"))
        //         y = textCom.transform.localPosition.y + rectTrans.rect.yMax - textCom.preferredHeight / 2f;
        //     else if(textCom.alignment.ToString().Contains("Lower"))
        //         y = textCom.transform.localPosition.y + rectTrans.rect.yMin + textCom.preferredHeight / 2f;

        //     return new Vector3(x, y, 0);
        // }
    }
}
