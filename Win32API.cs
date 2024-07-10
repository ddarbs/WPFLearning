using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace WPFLearning
{
    public class Win32API
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowText(IntPtr _hWnd, StringBuilder _text, int _count);
        
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string _class, string _title);
        
        
        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(IntPtr _hWnd, uint _msg, IntPtr _wParam, IntPtr _lParam);
        
        [DllImport("user32.dll")]
        public static extern Int32 PostMessage(IntPtr _hWnd, uint _msg, IntPtr _wParam, IntPtr _lParam);
        
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr _hWnd, int _id, uint _fsModifiers, uint _vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr _hWnd, int _id);
        
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr _hWnd);
        
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(uint virtualKeyCode);

    #region https://stackoverflow.com/questions/65610121/how-to-get-cursor-pos-in-a-specific-windows
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point) {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);
    #endregion
    
        public static WindowData GetActiveWindow()
        {
            IntPtr _DesiredHandle = GetForegroundWindow();

            if (_DesiredHandle == default)
            {
                Debug.WriteLine("[ERROR] No foreground window found");
                return null;
                //throw new NullReferenceException(); // don't want it to break, can handle it elsewhere
            }
            
            const int l_MaxLength = 256;
            StringBuilder l_Buffer = new StringBuilder(l_MaxLength);
            
            if (GetWindowText(_DesiredHandle, l_Buffer, l_MaxLength) == 0)
            {
                Debug.WriteLine("[WARNING] Foreground window has no title, double check it's correct");
                return WindowData.New(_DesiredHandle, "");
            }
            
            return WindowData.New(_DesiredHandle, l_Buffer.ToString());
        }

        public static WindowData GetWindowByTitle(string _title)
        {
            IntPtr _DesiredHandle = FindWindow(null, _title);
            
            if (_DesiredHandle == default)
            {
                Debug.WriteLine("[ERROR] Window not found");
                return null;
                //throw new NullReferenceException(); // don't want it to break, can handle it elsewhere
            }
            
            return WindowData.New(_DesiredHandle, _title);
        }

        public static bool RegisterHotkey(IntPtr _hWnd, int _id, uint _vk)
        {
            return RegisterHotKey(_hWnd, _id, 0, _vk);
        }
        
        public static POINT GetMousePositionInWindow(IntPtr _hWnd) 
        {
            GetCursorPos(out POINT _LPPoint);
            ScreenToClient(_hWnd, ref _LPPoint);
            return _LPPoint;
        }
    }
}
