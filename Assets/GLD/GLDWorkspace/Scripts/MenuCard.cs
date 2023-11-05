using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Takasho.GLD.VGS
{
    public class MenuCard : MonoBehaviour
    {
        [SerializeField] private Text _cardName;
        [SerializeField] private Image _cardImage;
        [SerializeField] private Color _mainColor;
        [SerializeField] private Color _accentColor;
        private Image _buttonImage;

        private void Awake()
        {
            _buttonImage = GetComponent<Image>();
        }

        public void UpdateContent(BaseDataSO data)
        {
            _cardName.text = data.DataName.Replace(" ", "\r\n");
            _cardImage.sprite = data.DataSprite;
        }

        public void PointerEnter()
        {
            _cardName.color = _mainColor;
            _buttonImage.color = _accentColor;
        }

        public void PointerExit()
        {
            _cardName.color = _accentColor;
            _buttonImage.color = _mainColor;
        }
    }
}

