using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Takasho.GLD.VGS
{
    public class ProductSideMenu : MonoBehaviour
    {

        [SerializeField] private Button _deleteButton;
        [SerializeField] private Text _sideMenuTitle;
        [SerializeField] private Transform _productInfoMenu;
        [SerializeField] private Transform _productColorMenu;
        [SerializeField] private Transform _productScaleMenu;
        [SerializeField] private Transform _productRotationMenu;

        public enum SideMenuState
        {
            COLOR,
            INFO,
            SCALE,
            ROTATION,
        }

        public void ShowMenu()
        {
            gameObject.SetActive(true);
        }

        public void HideMenu()
        {
            gameObject.SetActive(false);
        }

        public void SetDeleteButton(Action onDeleteButtonClicked)
        {
            _deleteButton.onClick.RemoveAllListeners();
            _deleteButton.onClick.AddListener(() => onDeleteButtonClicked());
        }

        public void UpdateProductSideMenu(SideMenuState _currentMenu)
        {
            _productInfoMenu.gameObject.SetActive(false);
            _productColorMenu.gameObject.SetActive(false);
            _productScaleMenu.gameObject.SetActive(false);
            _productRotationMenu.gameObject.SetActive(false);

            switch (_currentMenu)
            {
                case SideMenuState.INFO:
                    _sideMenuTitle.text = "詳細";
                    _productInfoMenu.gameObject.SetActive(true);
                    break;
                case SideMenuState.COLOR:
                    _sideMenuTitle.text = "カラー";
                    _productColorMenu.gameObject.SetActive(true);
                    break;
                case SideMenuState.SCALE:
                    _sideMenuTitle.text = "サイズ";
                    _productScaleMenu.gameObject.SetActive(true);
                    break;
                case SideMenuState.ROTATION:
                    _sideMenuTitle.text = "回転";
                    _productRotationMenu.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

    }
}