namespace WPFLearning
{
    internal static class Constants
    {
    #region Window Messages
        public const uint WM_ACTIVATEAPP = 0x001C;
        public const uint WM_NCACTIVATE = 0x0086;
        public const uint WM_IME_SETCONTEXT = 0x0281;
        public const uint WM_SETFOCUS = 0x0007;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
    #endregion Window Messages
    
    #region Virtual Keys
        public const IntPtr VK_KEY_F2 = 0x71;
        public const IntPtr VK_KEY_F3 = 0x72;
        public const IntPtr VK_KEY_F4 = 0x73;
        public const IntPtr VK_KEY_F5 = 0x74;
        public const IntPtr VK_LSHIFT = 0xA0;
        public const IntPtr VK_KEY_E = 0x45;
        public const IntPtr VK_MBUTTON = 0x04;
        public const IntPtr VK_KEY_XBUTTON2 = 0x06;
    #endregion Virtual Keys

    #region Hotkey IDs
        public const int HK_PAUSEBINDINGS = 0;
        public const int HK_SWITCHMACRO = 1;
        public const int HK_USEMACRO = 2;
        public const int HK_AUTORUN = 3;
    #endregion Hotkey IDs
    }
}
