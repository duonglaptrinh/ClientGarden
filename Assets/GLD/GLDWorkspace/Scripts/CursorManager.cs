using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Takasho.GLD.VGS
{

    public class CursorManager : MonoBehaviour
    {
        [SerializeField] private Texture2D _defaultCursor;
        [SerializeField] private Texture2D _rotateCursor;
        [SerializeField] private Texture2D _handCursor;
        [SerializeField] private Texture2D _productCursor;

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
            Cursor.SetCursor(_productCursor, Vector2.zero, CursorMode.Auto);
        }


    }
}
