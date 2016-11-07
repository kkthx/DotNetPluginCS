﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DotNetPlugin.SDK
{
    public class Bridge
    {
        public const int GUI_MAX_LINE_SIZE = 65536;
        public const int MAX_LABEL_SIZE = 256;
        public const int MAX_COMMENT_SIZE = 512;
        public const int MAX_MODULE_SIZE = 256;
        public const int MAX_IMPORT_SIZE = 65536;
        public const int MAX_BREAKPOINT_SIZE = 256;
        public const int MAX_CONDITIONAL_EXPR_SIZE = 256;
        public const int MAX_CONDITIONAL_TEXT_SIZE = 256;
        public const int MAX_SCRIPT_LINE_SIZE = 2048;
        public const int MAX_THREAD_NAME_SIZE = 256;
        public const int MAX_WATCH_NAME_SIZE = 256;
        public const int MAX_STRING_SIZE = 512;
        public const int MAX_ERROR_SIZE = 512;
        public const int MAX_SECTION_SIZE = 10;
        public const int MAX_COMMAND_LINE_SIZE = 256;
        public const int MAX_MNEMONIC_SIZE = 64;
        public const int PAGE_SIZE = 4096;

#if AMD64
        [DllImport("x64bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GuiGetLineWindow(string title, ref IntPtr text);

        [DllImport("x64bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DbgValFromString(string Sstring);

        [DllImport("x64bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DbgGetModuleAt(IntPtr addr, IntPtr text);

        [DllImport("x64bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DbgModBaseFromName(string name);

        [DllImport("x64bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DbgIsDebugging();

        [DllImport("x64bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DbgCmdExec(string cmd);

        [DllImport("x64bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BridgeAlloc(IntPtr size);

        [DllImport("x64bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BridgeFree(IntPtr size);
#else
        [DllImport("x32bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GuiGetLineWindow(string title, ref IntPtr text);

        [DllImport("x32bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DbgValFromString(string Sstring);

        [DllImport("x32bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DbgGetModuleAt(IntPtr addr, IntPtr text);

        [DllImport("x32bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DbgModBaseFromName(string name);

        [DllImport("x32bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DbgIsDebugging();

        [DllImport("x32bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DbgCmdExec(string cmd);

        [DllImport("x32bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BridgeAlloc(IntPtr size);

        [DllImport("x32bridge.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BridgeFree(IntPtr size);
#endif

        public struct ICONDATA
        {
            public IntPtr data;
            public long size;
        }

        public struct ListInfo
        {
            public int count;
            public IntPtr size;
            public IntPtr data;

            public T[] ToArray<T>(bool success) where T : new()
            {
                if (!success || count == 0 || size == IntPtr.Zero)
                    return new T[0];
                var list = new T[count];
                var szt = Marshal.SizeOf(typeof(T));
                var sz = size.ToInt32() / count;
                if (szt != sz)
                    throw new InvalidDataException(string.Format("{0} type size mismatch, expected {1} got {2}!",
                        typeof(T).Name, szt, sz));
                var ptr = data;
                for (var i = 0; i < count; i++)
                {
                    list[i] = (T)Marshal.PtrToStructure(ptr, typeof(T));
                    ptr += sz;
                }
                BridgeFree(data);
                return list;
            }
        }
    }
}
