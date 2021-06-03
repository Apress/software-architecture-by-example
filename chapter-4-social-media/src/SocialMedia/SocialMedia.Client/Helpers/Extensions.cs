using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialMedia.Client.Helpers
{
    public static class Extensions
    {
        private static Random _random = new Random();

        public static T GetRandom<T>(this List<T> list)
        {
            int idx = _random.Next(list.Count);
            return list.ElementAt(idx);
        }
    }
}
