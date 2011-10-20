using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PngtoFshBatchtxt
{
    static class ListExtensions
    {
        public static void SetCapacity(this List<string> list, int newCapacity)
        {
            if (list.Capacity < newCapacity)
            {
                list.Capacity = newCapacity;
            }
        }
        public static void SetCapacity(this List<BatchFshContainer> list, int newCapacity)
        {
            if (list.Capacity < newCapacity)
            {
                list.Capacity = newCapacity;
            }
        }


    }
}
