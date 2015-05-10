using System;

namespace Rogero.RxLoops.Tests
{
    public static class BddStringExtensions
    {
        public static void _(this string s, Action action)
        {
            action();
        }
    }
}