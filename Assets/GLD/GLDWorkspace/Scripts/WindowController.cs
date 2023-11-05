using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takasho.GLD.VGS
{
    public class WindowController : MonoBehaviour
    {
        [SerializeField] private Transform _window;
        [SerializeField] Vector3 distance = new Vector3(0f, 0f, 0.8f);

        Vector3 targetPos;
        Vector3 startPos;


        private bool _isOpen = false;
        public bool _isClose
        {
            get { return _isOpen; }
            set { _isOpen = !value; }
        }
        

        public float moveTime = 1.0f;  // �ړ�����
        float elapsedTime = 0f;         // �o�ߎ���
        float rate;                     // ����

        bool isMoving = false;
        private void Start()
        {

            startPos = _window.transform.localPosition;
            targetPos = startPos + distance;


        }


        /// <summary>
        /// need refactoring
        /// </summary>
        private void Update()
        {
            //if (isMoving == false) return;
            //// �o�ߎ��Ԃ��߂����Ƃ��̏���
            //if (elapsedTime >= moveTime)
            //{
            //    //Debug.Log(elapsedTime);
            //    //Debug.Log(moveTime);
            //    isMoving = false;
            //    return;
            //}

            elapsedTime += Time.deltaTime;  // �o�ߎ��Ԃ̉��Z
            rate = Mathf.Clamp01(elapsedTime / moveTime);   // �����v�Z

            if (_isOpen )
            {

                _window.localPosition = Vector3.Lerp(startPos, targetPos, rate);
            }
            else
            {

                _window.localPosition = Vector3.Lerp(targetPos, startPos, rate);

            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isMoving = true;
                elapsedTime = 0f;
                _isOpen = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isMoving = true;
                elapsedTime = 0f;
                _isOpen = false;
            }
        }

    }
}
