using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Takasho.GLD.VGS
{
    public class ProductEditMenu : MonoBehaviour
    {
        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _colorButton;
        [SerializeField] private Button _scaleButton;
        [SerializeField] private Button _rotationButton;
        [SerializeField] private Button _trashButton;
        [SerializeField] private Button _deleteButton;

        public void ShowMenu()
        {
            gameObject.SetActive(true);
        }

        public void HideMenu()
        {
            gameObject.SetActive(false);
        }

        public void SetColorButton(Action onColorButtonClicked)
        {
            _colorButton.onClick.RemoveAllListeners();
            _colorButton.onClick.AddListener(() => onColorButtonClicked());
        }
        public void SetInfoButton(Action onInfoButtonClicked)
        {
            _infoButton.onClick.RemoveAllListeners();
            _infoButton.onClick.AddListener(() => onInfoButtonClicked());
        }
        public void SetScaleButton(Action onScaleButtonClicked)
        {
            _scaleButton.onClick.RemoveAllListeners();
            _scaleButton.onClick.AddListener(() => onScaleButtonClicked());
        }
        public void SetRotationButton(Action onRotationButtonClicked)
        {
            _rotationButton.onClick.RemoveAllListeners();
            _rotationButton.onClick.AddListener(() => onRotationButtonClicked());
        }
        public void SetDeleteButton(Action onDeleteButtonClicked)
        {
            _deleteButton.onClick.RemoveAllListeners();
            _deleteButton.onClick.AddListener(() => onDeleteButtonClicked());
        }
    }
}

