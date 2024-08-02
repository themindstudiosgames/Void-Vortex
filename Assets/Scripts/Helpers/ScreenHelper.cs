using UnityEngine;

namespace Helpers
{
    public static class ScreenHelper
    {
        public static bool IsPortrait()
        {
            return Screen.height > Screen.width;
        }
    }
}