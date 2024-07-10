using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
            
            MainMacro.Initialize(Title);
        }

        private IntPtr WndProc(IntPtr _hWnd, int _msg, IntPtr _wParam, IntPtr _lParam, ref bool handled)
        {
            if (_msg != 0x0312) // WM_HOTKEY
            {
                return IntPtr.Zero; 
            }
            
            switch (_wParam.ToInt32())
            {
                case Constants.HK_PAUSEBINDINGS:
                    Debug.WriteLine("hotkey 0 (pause)");
                    // mainmacro pause
                    break;
                case Constants.HK_SWITCHMACRO:
                    SwitchMacro();
                    break;
                case Constants.HK_USEMACRO:
                    Debug.WriteLine("hotkey 2 (use)");
                    MainMacro.UseMacro();
                    break;
                case Constants.HK_AUTORUN:
                    Debug.WriteLine("hotkey 3 (autorun)");
                    MainMacro.UseMacro();
                    break;
            }

            return IntPtr.Zero;
        }
        
        private void Button_FindWindow(object sender, RoutedEventArgs e)
        {
            if (inptTitleSearch.Text == "")
            {
                return;
            }

            if (!MainMacro.FindWindowByTitle(inptTitleSearch.Text))
            {
                txtWindowTitle.Text = txtWindowHandle.Text = "[not found]";
                btnLockWindow.IsEnabled = false;
                btnSwitchMacro.IsEnabled = false;
                btnUseMacro.IsEnabled = false;
                return;
            }
            
            txtWindowHandle.Text = MainMacro.GetWindowHandle().ToString();
            txtWindowTitle.Text = inptTitleSearch.Text;

            btnLockWindow.IsEnabled = true;
        }
        
        private void Button_LockWindow(object sender, RoutedEventArgs e)
        {
            if (!MainMacro.HaveWindowData())
            {
                return;
            }
            
            inptTitleSearch.IsEnabled = !inptTitleSearch.IsEnabled;
            btnFindWindow.IsEnabled = !btnFindWindow.IsEnabled;
            btnLockWindow.Content = btnFindWindow.IsEnabled switch
            {
                true => "Lock Window",
                false => "Unlock Window"
            };
            boxMacroSprint.IsEnabled = !boxMacroSprint.IsEnabled;
            boxMacroResource.IsEnabled = !boxMacroResource.IsEnabled;
            boxMacroPickUp.IsEnabled = !boxMacroPickUp.IsEnabled;
            boxMacroDump.IsEnabled = !boxMacroDump.IsEnabled;
            
            MainMacro.ChangeWindowLock(!btnFindWindow.IsEnabled);
            CheckEnabledMacros();
            
            MainMacro.RegisterHotkeys();
        }
        
        private void Button_SwitchMacro(object sender, RoutedEventArgs e)
        {
            SwitchMacro();
        }

        private void SwitchMacro()
        {
            if (!MainMacro.HaveWindowData() || !MainMacro.GetWindowLock())
            {
                return;
            }
            
            MainMacro.SwitchMacro();
            txtCurrentMacro.Text = MainMacro.GetCurrentMacro();
        }
        
        private void Button_UseMacro(object sender, RoutedEventArgs e)
        {
            if (!MainMacro.HaveWindowData() || !MainMacro.GetWindowLock())
            {
                return;
            }
            
            MainMacro.UseMacro();
        }

        private void CheckBox_Sprint(object sender, RoutedEventArgs e)
        {
            CheckBox _Sender = (CheckBox)sender;
            switch (_Sender.IsChecked)
            {
                case true:
                    MainMacro.EnableMacro("Sprint");
                    break;
                case false:
                    MainMacro.DisableMacro("Sprint");
                    break;
            }
            
            CheckEnabledMacros();
        }
        
        private void CheckBox_Resource(object sender, RoutedEventArgs e)
        {
            CheckBox _Sender = (CheckBox)sender;
            switch (_Sender.IsChecked)
            {
                case true:
                    MainMacro.EnableMacro("Resource");
                    break;
                case false:
                    MainMacro.DisableMacro("Resource");
                    break;
            }
            
            CheckEnabledMacros();
        }
        
        private void CheckBox_PickUp(object sender, RoutedEventArgs e)
        {
            CheckBox _Sender = (CheckBox)sender;
            switch (_Sender.IsChecked)
            {
                case true:
                    MainMacro.EnableMacro("PickUp");
                    break;
                case false:
                    MainMacro.DisableMacro("PickUp");
                    break;
            }

            CheckEnabledMacros();
        }
        
        private void CheckBox_Dump(object sender, RoutedEventArgs e)
        {
            CheckBox _Sender = (CheckBox)sender;
            switch (_Sender.IsChecked)
            {
                case true:
                    MainMacro.EnableMacro("Dump");
                    break;
                case false:
                    MainMacro.DisableMacro("Dump");
                    break;
            }

            CheckEnabledMacros();
        }

        private void CheckEnabledMacros()
        {
            if (!MainMacro.GetWindowLock())
            {
                btnSwitchMacro.IsEnabled = false;
                btnUseMacro.IsEnabled = false;
                if(MainMacro.GetEnabledMacroCount() == 0)
                {
                    return;
                }
                txtCurrentMacro.Text = MainMacro.GetCurrentMacro();
                return;
            }
            if (MainMacro.GetEnabledMacroCount() >= 2)
            {
                btnSwitchMacro.IsEnabled = true;
                btnUseMacro.IsEnabled = true;
                txtCurrentMacro.Text = MainMacro.GetCurrentMacro();
                return;
            }
            if (MainMacro.GetEnabledMacroCount() == 1)
            {
                btnSwitchMacro.IsEnabled = false;
                btnUseMacro.IsEnabled = true;
                txtCurrentMacro.Text = MainMacro.GetCurrentMacro();
                return;
            }
            
            btnSwitchMacro.IsEnabled = false;
            btnUseMacro.IsEnabled = false;
            txtCurrentMacro.Text = "[no macros enabled]";
        }
        
        private void Button_Test(object sender, RoutedEventArgs e)
        {
            Mouse.Capture(this, CaptureMode.SubTree);
            AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("mouse click");
            ReleaseMouseCapture();
        }
    }
}