using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Keyboard
{
    public sealed class KeyboardNode : Node<EmptyConfiguration, EmptyEnvironment>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => CommonNodeDescription.Create<KeyboardNode>(
                Guid.Parse(KeyboardPlugin.PluginId),
                "Keyboard",
                "Blabla")
            .AddOutputDescription(KeyOutputDescription);

        /// <summary>
        /// Description of the keyboard output connector
        /// </summary>
        public static ConnectorDescription KeyOutputDescription => new ConnectorDescription(
            "KeyOutput", "key Output", "Key output signal",
            ComplexContentType.Create<int>(Guid.Parse(KeyboardPlugin.PluginId)));

        #endregion

        private readonly IOutputConnectorHandler _outputConnector;

        internal readonly GlobalKeyboardHook _hook;
        internal readonly KeyboardHelper _keyboard;

        protected override EmptyConfiguration DefaultConfiguration => new EmptyConfiguration();

        public KeyboardNode(Guid id) : base(id)
        {
            _hook = new GlobalKeyboardHook();
            _hook.KeyboardPressed += HookOnKeyboardPressed;

            _keyboard = new KeyboardHelper(CultureInfo.CurrentCulture);

            _outputConnector = RegisterOutputConnector<int>(KeyOutputDescription);
        }

        private void HookOnKeyboardPressed(object sender, GlobalKeyboardHook.GlobalKeyboardHookEventArgs e)
        {
            if (State == NodeState.Running)
            {
                _outputConnector.Send(e.KeyboardData.VirtualCode);
            }
        }

        protected override Task OnInitialize(EmptyConfiguration configuration, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnDeinitialize(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnInitializeReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override void OnDispose()
        {
            _hook.Dispose();
            _keyboard.Dispose();
        }
    }

    internal class KeyboardHelper : IDisposable
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

        public KeyboardHelper(CultureInfo cultureInfo)
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

        ~KeyboardHelper()
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

    //Based on https://gist.github.com/Stasonix
    internal class GlobalKeyboardHook : IDisposable
    {
        public event EventHandler<GlobalKeyboardHookEventArgs> KeyboardPressed;

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
        /// You would install a hook procedure to monitor the system for certain types of events. These events are
        /// associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">hook type</param>
        /// <param name="lpfn">hook procedure</param>
        /// <param name="hMod">handle to application instance</param>
        /// <param name="dwThreadId">thread identifier</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport("USER32", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk">handle to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("USER32", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        /// A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hHook">handle to current hook</param>
        /// <param name="code">hook code passed to hook procedure</param>
        /// <param name="wParam">value passed to hook procedure</param>
        /// <param name="lParam">value passed to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("USER32", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;

        private IntPtr _windowsHookHandle;
        private IntPtr _user32LibraryHandle;
        private HookProc _hookProc;

        public GlobalKeyboardHook()
        {
            _windowsHookHandle = IntPtr.Zero;
            _user32LibraryHandle = IntPtr.Zero;

            // we must keep alive _hookProc, because GC is not aware about SetWindowsHookEx behaviour.
            _hookProc = LowLevelKeyboardProc;

            _user32LibraryHandle = LoadLibrary("User32");
            if (_user32LibraryHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode,
                    "Failed to load library 'User32.dll'. " +
                    $"Error {errorCode}: {new Win32Exception(errorCode).Message}.");
            }

            _windowsHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0);
            if (_windowsHookHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode,
                    $"Failed to adjust keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. " +
                    $"Error {errorCode}: {new Win32Exception(errorCode).Message}.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // because we can unhook only in the same thread, not in garbage collector thread
                if (_windowsHookHandle != IntPtr.Zero)
                {
                    if (!UnhookWindowsHookEx(_windowsHookHandle))
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode,
                            $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. " +
                            $"Error {errorCode}: {new Win32Exception(errorCode).Message}.");
                    }

                    _windowsHookHandle = IntPtr.Zero;

                    // ReSharper disable once DelegateSubtraction
                    _hookProc -= LowLevelKeyboardProc;
                }
            }

            if (_user32LibraryHandle != IntPtr.Zero)
            {
                // reduces reference to library by 1.
                if (!FreeLibrary(_user32LibraryHandle))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode,
                        $"Failed to unload library 'User32.dll'. " +
                        $"Error {errorCode}: {new Win32Exception(errorCode).Message}.");
                }

                _user32LibraryHandle = IntPtr.Zero;
            }
        }

        ~GlobalKeyboardHook()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var handled = false;
            var wparamTyped = wParam.ToInt32();

            if (Enum.IsDefined(typeof(KeyboardState), wparamTyped))
            {
                var keyboardEvent = Marshal.PtrToStructure<LowLevelKeyboardInputEvent>(lParam);
                var eventArguments = new GlobalKeyboardHookEventArgs(keyboardEvent, (KeyboardState)wparamTyped);

                KeyboardPressed?.Invoke(this, eventArguments);

                handled = eventArguments.Handled;
            }

            return handled ? (IntPtr)1 : CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LowLevelKeyboardInputEvent
        {
            /// <summary>
            /// A virtual-key code. The code must be a value in the range 1 to 254.
            /// </summary>
            public int VirtualCode;

            /// <summary>
            /// A hardware scan code for the key.
            /// </summary>
            public int HardwareScanCode;

            /// <summary>
            /// The extended-key flag, event-injected Flags, context code, and transition-state flag.
            /// This member is specified as follows. An application can use the following values
            /// to test the keystroke Flags. Testing LLKHF_INJECTED (bit 4) will tell you whether
            /// the event was injected. If it was, then testing LLKHF_LOWER_IL_INJECTED (bit 1) will tell
            /// you whether or not the event was injected from a process running at lower integrity level.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The time stamp for this message, equivalent to what GetMessageTime would return for this message.
            /// </summary>
            public int TimeStamp;

            /// <summary>
            /// Additional information associated with the message.
            /// </summary>
            public IntPtr AdditionalInformation;
        }

        public enum KeyboardState
        {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            SysKeyDown = 0x0104,
            SysKeyUp = 0x0105
        }

        public class GlobalKeyboardHookEventArgs : HandledEventArgs
        {
            public KeyboardState KeyboardState { get; }
            public LowLevelKeyboardInputEvent KeyboardData { get; }

            public GlobalKeyboardHookEventArgs(
                LowLevelKeyboardInputEvent keyboardData,
                KeyboardState keyboardState)
            {
                KeyboardData = keyboardData;
                KeyboardState = keyboardState;
            }
        }
    }
}