using System.Linq;

namespace CGServicesHV.Common.Utilities
{
    public static class InExtension
    {
        public static bool In<T>(this T x, params T[] args)
        {
            return args.Contains(x);
        }
    }
}
