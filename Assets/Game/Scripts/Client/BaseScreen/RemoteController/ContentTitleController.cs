using TWT.Model;

namespace TWT.Client
{
    public class ContentTitleController : UiControllerBase<ContentTitleView>
    {
        protected override void Initialize()
        {
            var contentInfo = GameContext.ContentInfoCurrent;
            if (contentInfo != null)
            {
                View.ShowInfo(contentInfo);
            }
        }

        public void Init()
        {
            var contentInfo = GameContext.ContentInfoCurrent;
            if (contentInfo != null)
            {
                View.ShowInfo(contentInfo);
            }
        }

        public VrContentTitle GetVrContentTitle()
        {
            return View.GetContentTitle();
        }
    }
}