﻿/*
* This file is part of PngtoFshBatch, a tool for batch converting images
* to FSH.
*
* Copyright (C) 2009-2017, 2023 Nicholas Hayes
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*
*/

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace PngtoFshBatchtxt
{
    /* The following code was adapted from
     * http://remyblok.tweakblogs.net/blog/6606/win7-recent-jumplist-without-associating-a-filetype.html
     */
    static class JumpListHelper
    {
        private static class NativeMethods
        {
            [ComImport,
                Guid("000214F9-0000-0000-C000-000000000046"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            internal interface IShellLinkW { }

            [DllImport("shell32.dll", ExactSpelling = true)]
            internal static extern void SHAddToRecentDocs(ShellAddToRecentDocs flags, [MarshalAs(UnmanagedType.Interface)] IShellLinkW link);
        }

        internal enum ShellAddToRecentDocs : uint
        {
            Pidl = 0x1,
            PathA = 0x2,
            PathW = 0x3,
            AppIdInfo = 0x4,       // indicates the data type is a pointer to a SHARDAPPIDINFO structure
            AppIdInfoIdList = 0x5, // indicates the data type is a pointer to a SHARDAPPIDINFOIDLIST structure
            Link = 0x6,            // indicates the data type is a pointer to an IShellLink instance
            AppIdInfoLink = 0x7,   // indicates the data type is a pointer to a SHARDAPPIDINFOLINK structure 
        }

        private static MethodInfo nativeShellLinkGetMethod;
        public static void AddToRecent(JumpListLink link)
        {
            if (nativeShellLinkGetMethod == null)
            {
                //find the NativeShellLink property on the JumpListLink
                PropertyInfo nativeShellLinkProperty = typeof(JumpListLink).GetProperty("NativeShellLink", BindingFlags.Instance | BindingFlags.NonPublic);

                if (nativeShellLinkProperty == null)
                    throw new InvalidOperationException();

                //Save the Method info for later use, so we have to do the reflection only once.
                nativeShellLinkGetMethod = nativeShellLinkProperty.GetGetMethod(true);
            }

            //Get the value of the NativeShellLink property.
            //Cast this to our own implementation of IShellLinkW because it is using COM interop.
            NativeMethods.IShellLinkW nativeShellLink = (NativeMethods.IShellLinkW)nativeShellLinkGetMethod.Invoke(link, null);

            // Now make the call to Win32 to add the link to the recent items
            NativeMethods.SHAddToRecentDocs(ShellAddToRecentDocs.Link, nativeShellLink);
        }
        
    }
}
