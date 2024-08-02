namespace Helpers
{
    public static class PlatformHelper
    {
        public static bool IsEditor =>
#if UNITY_EDITOR
            true;
#else
            false;
#endif
    }
}