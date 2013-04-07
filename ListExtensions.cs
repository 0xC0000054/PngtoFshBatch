using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PngtoFshBatchtxt
{
    static class ListExtensions
    {
        public static void SetCapacity<T>(this List<T> list, int newCapacity)
        {
            if (list.Capacity < newCapacity)
            {
                list.Capacity = newCapacity;
            }
        }
    }
}
