using Shim.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIScroll
{
    public class UIScrollGrid : UIScrollBase
    {
        public int col;
        protected override void Start()
        {
            //Initialize(new List<ItemDataBase>() {
            //new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(),
            // new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(),
            //  new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase()
            //}, col); // Test Only
        }

        public virtual void Initialize(List<ItemDataBase> list, int col, Transform otherParent = null, float moreContent = 0)
        {
            this.col = col;
            Initialize(list, otherParent, moreContent);
        }

        protected override void CalculateFitContent(float moreContent = 0, int count = 0)
        {
            float spacing = 0;
            RectOffset pading = new RectOffset(0, 0, 0, 0);
            GridLayoutGroup grid = scroll.content.GetComponent<GridLayoutGroup>();
            if (grid)
            {
                pading = grid.padding;
                spacing = grid.spacing.y;
                grid.constraintCount = col;
            }

            RectTransform rectContent = (RectTransform)scroll.content.transform;

            float n = listDatas.Count;
            int add = n % col == 0 ? 0 : 1;

            rectContent.sizeDelta = new Vector2(rectContent.rect.width, pading.top + pading.bottom + (grid.cellSize.y + spacing) * (n / col + add) + moreContent);

            RectTransformExtensions.SetLeft(rectContent, 0);
            RectTransformExtensions.SetRight(rectContent, 0);
        }
        public override void FocusItem(RectTransform rect)
        {
            Vector3 v = scroll.content.localPosition;
            v.y = rect.localPosition.y;
            DebugExtension.LogError(rect.localPosition.y);
            //RectTransformExtensions.SetTop(scroll.content, -rect.offsetMax.y);

            scroll.content.localPosition = v;
        }
    }
}