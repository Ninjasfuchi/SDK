namespace AdSystem.Scripts
{
    public static class AdServices
    {
        public static IAdService Current { get; private set; }

        public static bool IsRegistered => Current != null;

        public static void Register(IAdService service)
        {
            Current = service;
        }

        public static void Unregister(IAdService service)
        {
            if (Current == service)
                Current = null;
        }
    }
}