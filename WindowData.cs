namespace WPFLearning
{
    public class WindowData
    {
        public IntPtr p_Handle { get; private set; } = IntPtr.Zero;
        public string p_Title { get; private set; } = string.Empty;

        private WindowData(IntPtr _handle, string _title)
        {
            this.p_Handle = _handle;
            this.p_Title = _title;
        }

        public void IsDifferentThan(WindowData _otherWindow, Action _onDifferent)
        {
            if (_otherWindow.p_Handle == this.p_Handle)
            {
                return;
            }
            _onDifferent();
        }

        public static WindowData New(IntPtr _handle, string _title) => new WindowData(_handle, _title);
    } 
}