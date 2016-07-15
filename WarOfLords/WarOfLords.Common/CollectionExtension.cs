using System;
using System.Collections.Generic;
using System.Text;
using WarOfLords.Common.Models;

namespace WarOfLords.Common
{
    public static class CollectionExtension
    {
        public static void ForEach<T>(this List<T> list, Action<T> action)
        {
            foreach(var unit in list)
            {
                action(unit);
            }
        }
    }
}
