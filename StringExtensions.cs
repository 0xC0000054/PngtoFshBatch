﻿using System;

namespace PngtoFshBatchtxt
{
    internal static class StringExtensions
    {
        internal static bool Contains(this string s, string value, StringComparison comparisonType)
        {
            return s.IndexOf(value, comparisonType) >= 0;
        }
    }
}
