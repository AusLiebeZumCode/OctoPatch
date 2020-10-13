using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace OctoPatch.Plugin.Keyboard
{
    /// <summary>
    /// Helper class to map virtual keys from the keyboard to usable output
    /// </summary>
    internal class KeyboardTranslator : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern bool UnloadKeyboardLayout(IntPtr hkl);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        private readonly IntPtr _pointer;

        public KeyboardTranslator(CultureInfo cultureInfo)
        {
            _pointer = LoadKeyboardLayout(cultureInfo.KeyboardLayoutId.ToString("X8"), 1);

            if (_pointer == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public string GetUnicodeFromVirtualKeyCode(uint virtualKeyCode)
        {
            var keyboardState = new byte[256];
            if (!GetKeyboardState(keyboardState))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            var scanCode = MapVirtualKeyEx(virtualKeyCode, (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC, _pointer);
            var builder = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, builder, 5, 0, _pointer);

            return builder.ToString();
        }

        public char GetCharFromVirtualKeyCode(uint virtualKeyCode)
        {
            return (char)MapVirtualKeyEx(virtualKeyCode, (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR, _pointer);
        }

        public void Dispose()
        {
            UnloadKeyboardLayout(_pointer);
            GC.SuppressFinalize(this);
        }

        ~KeyboardTranslator()
        {
            UnloadKeyboardLayout(_pointer);
        }

        /// <summary>
        /// The set of valid MapTypes used in MapVirtualKey
        /// </summary>
        private enum MapVirtualKeyMapTypes : uint
        {
            /// <summary>
            /// uCode is a virtual-key code and is translated into a scan code.
            /// If it is a virtual-key code that does not distinguish between left- and
            /// right-hand keys, the left-hand scan code is returned.
            /// If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_VSC = 0x00,

            /// <summary>
            /// uCode is a scan code and is translated into a virtual-key code that
            /// does not distinguish between left- and right-hand keys. If there is no
            /// translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK = 0x01,

            /// <summary>
            /// uCode is a virtual-key code and is translated into an unshifted
            /// character value in the low-order word of the return value. Dead keys (diacritics)
            /// are indicated by setting the top bit of the return value. If there is no
            /// translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_CHAR = 0x02,

            /// <summary>
            /// Windows NT/2000/XP: uCode is a scan code and is translated into a
            /// virtual-key code that distinguishes between left- and right-hand keys. If
            /// there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK_EX = 0x03,

            /// <summary>
            /// Not currently documented
            /// </summary>
            MAPVK_VK_TO_VSC_EX = 0x04
        }
    }
}