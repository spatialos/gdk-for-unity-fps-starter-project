using UnityEngine;

namespace Fps
{
    public class StartScreenManager : ConnectionStatusManager
    {
        public FpsUIButton QuickJoinButton;
        public FpsUIButton BrowseButton;

        private void OnValidate()
        {
            if (QuickJoinButton == null)
            {
                throw new MissingReferenceException("Missing reference to the quick join button.");
            }

            if (BrowseButton == null)
            {
                throw new MissingReferenceException("Missing reference to the browse button.");
            }
        }
    }
}
