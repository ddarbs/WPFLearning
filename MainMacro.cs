using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFLearning
{
    /* TODO: 
     * - add autorun
     * - add dump mode clicking at current cursor pos
     * - (wip) add monitoring for pressing MMB/F4 out of focus of either window
     * - add emergency pause/stop
     * - (wip) change stuff to use keybinds
     * - add changeable keybinds for ingame stuff like autorun, pickup, etc
     */

    public class MainMacro
    {
        private static List<string> i_BaseMacros = new List<string>()
        {
            "Sprint",
            "Resource",
            "PickUp",
            "Dump",
        };

        private static List<string> i_EnabledMacros = new List<string>();

        private static WindowData? i_HelperWindowData;
        private static WindowData? i_PaxWindowData;
        private static bool i_HaveWindowData = false;
        private static bool i_WindowLocked = false;
        private static bool i_HotkeysRegistered = false;
        private static bool i_PauseHotkeyRegistered = false;
        private static bool i_SwitchHotkeyRegistered = false;
        private static bool i_UseHotkeyRegistered = false;
        private static bool i_AutorunHotkeyRegistered = false;
        private static int i_CurrentMacro = 0;
        private static bool i_Looping = false;

        public static void Initialize(string _title)
        {
            i_HelperWindowData = Win32API.GetWindowByTitle(_title);
        }
        
        public static void RegisterHotkeys()
        {
            if (i_HotkeysRegistered)
            {
                return;
            }
            
            i_PauseHotkeyRegistered = Win32API.RegisterHotkey(i_HelperWindowData.p_Handle, Constants.HK_PAUSEBINDINGS, (uint)Constants.VK_KEY_F2);
            i_SwitchHotkeyRegistered = Win32API.RegisterHotkey(i_HelperWindowData.p_Handle, Constants.HK_SWITCHMACRO, (uint)Constants.VK_KEY_F3);
            i_UseHotkeyRegistered = Win32API.RegisterHotkey(i_HelperWindowData.p_Handle, Constants.HK_USEMACRO, (uint)Constants.VK_KEY_F4);
            i_AutorunHotkeyRegistered = Win32API.RegisterHotkey(i_HelperWindowData.p_Handle, Constants.HK_AUTORUN, (uint)Constants.VK_MBUTTON);
            
            if (i_PauseHotkeyRegistered && i_SwitchHotkeyRegistered && i_UseHotkeyRegistered && i_AutorunHotkeyRegistered)
            {
                i_HotkeysRegistered = true;
            }
            Debug.WriteLine("register result: " + i_HotkeysRegistered);
        }
        
        public static void EnableMacro(string _macro)
        {
            int _Index = Math.Clamp(i_BaseMacros.IndexOf(_macro), 0, i_EnabledMacros.Count);
            if (i_EnabledMacros.Count > 1 && i_BaseMacros.IndexOf(_macro) != 0)
            {
                if (i_BaseMacros.IndexOf(i_EnabledMacros[0]) > i_BaseMacros.IndexOf(_macro))
                {
                    _Index--;
                }
            }
            
            i_EnabledMacros.Insert(_Index, _macro);

            i_CurrentMacro = 0;
        }

        public static void DisableMacro(string _macro)
        {
            int _Index = i_BaseMacros.IndexOf(_macro);
            i_EnabledMacros.Remove(_macro);

            i_CurrentMacro = 0;
        }

        public static bool FindWindowByTitle(string _title)
        {
            WindowData _WindowData = Win32API.GetWindowByTitle(_title);

            if(_WindowData == null)
            {
                i_HaveWindowData = false;
                return false;
            }
            if (i_HaveWindowData)
            {
                if (!Win32API.IsWindow(i_PaxWindowData.p_Handle))
                {
                    return true;
                }
                
                i_PaxWindowData = default;
                i_HaveWindowData = false;
                return false;
            }
            
            i_PaxWindowData = _WindowData;
            i_HaveWindowData = true;
            return true;
        }

        public static bool HaveWindowData()
        {
            return i_HaveWindowData;
        }

        public static int GetEnabledMacroCount()
        {
            return i_EnabledMacros.Count;
        }
        
        public static IntPtr GetWindowHandle()
        {
            return i_PaxWindowData is null ? default : i_PaxWindowData.p_Handle;
        }

        public static string GetCurrentMacro()
        {
            return i_EnabledMacros[i_CurrentMacro];
        }

        /*public static IntPtr GetWindowHandleHex()
        {
            string l_Hex = i_WindowData.p_Handle.ToString("X");
            Debug.WriteLine("0x" + l_Hex);
            Debug.WriteLine(Convert.ToInt32(l_Hex, 16));
            return new IntPtr(Convert.ToInt32(l_Hex, 16));
        }*/

        public static void ChangeWindowLock(bool _locked)
        {
            i_WindowLocked = _locked;
        }
        
        public static bool GetWindowLock()
        {
            return i_WindowLocked;
        }

        public static void SwitchMacro()
        {
            i_Looping = false;
            if (i_CurrentMacro + 1 >= i_EnabledMacros.Count)
            {
                i_CurrentMacro = 0;
            }
            else
            {
                i_CurrentMacro++;
            }
        }

        public static void UseMacro() // TODO: change key presses to use the keybinds
        {
            if (i_PaxWindowData == null)
            {
                return;
            }
            
            switch (i_EnabledMacros[i_CurrentMacro])
            {
                case "Sprint":
                    Win32API.PostMessage(i_PaxWindowData.p_Handle, Constants.WM_KEYDOWN, Constants.VK_LSHIFT, 0);
                    break;
                case "Resource":
                    Resource();
                    break;
                case "PickUp":
                    if (i_Looping)
                    {
                        i_Looping = false;
                    }
                    else
                    {
                        PickupLoop();
                    }
                    break;
                case "Dump":
                    if (i_Looping)
                    {
                        i_Looping = false;
                    }
                    else
                    {
                        DumpLoop();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static async void Resource()
        {
            Win32API.SendMessage(i_PaxWindowData.p_Handle, Constants.WM_ACTIVATEAPP, 1, 0);
            await Task.Delay(10);
            //Win32API.SendMessage(i_WindowData.p_Handle, WM_NCACTIVATE, 1, 0);
            //await Task.Delay(10);
            //Win32API.SendMessage(i_WindowData.p_Handle, WM_IME_SETCONTEXT, 1, 0);
            //await Task.Delay(10);
            //Win32API.SendMessage(i_WindowData.p_Handle, WM_SETFOCUS, 1, 0);
            //await Task.Delay(10);
            Win32API.PostMessage(i_PaxWindowData.p_Handle, Constants.WM_LBUTTONUP, 0, MakeXYParam(500, 500));
            await Task.Delay(50);
            //Win32API.SendMessage(i_WindowData.p_Handle, WM_MOUSEMOVE, 0, MakeXYParam(500, 500));
            //await Task.Delay(10);
            Win32API.PostMessage(i_PaxWindowData.p_Handle, Constants.WM_LBUTTONDOWN, 1, MakeXYParam(500, 500));
            // maybe try sending the click down twice and see what changes in the messages between them?
        }
        
        private static async void PickupLoop()
        {
            i_Looping = true;
            while (i_Looping)
            {
                await Pickup();
            }
        } 

        private static async Task Pickup()
        {
            Win32API.PostMessage(i_PaxWindowData.p_Handle, Constants.WM_KEYDOWN, Constants.VK_KEY_E, 0);
            await Task.Delay(10);
            Win32API.PostMessage(i_PaxWindowData.p_Handle, Constants.WM_KEYUP, Constants.VK_KEY_E, 0);
            await Task.Delay(10);
        }

        private static async void DumpLoop()
        {
            Debug.Write("in dump loop");
            i_Looping = true;
            while (i_Looping)
            {
                await DumpAtMouse();
            }
        } 

        private static async Task DumpAtMouse() // TODO: fix this to ensure active window and grab current mouse pos
        {
            /*
            Win32API.SendMessage(i_PaxWindowData.p_Handle, Constants.WM_ACTIVATEAPP, 1, 0);
            await Task.Delay(10);
            */
            Point _MousePos = Win32API.GetMousePositionInWindow(i_PaxWindowData.p_Handle);
            Debug.WriteLine(_MousePos.X + ", " + _MousePos.Y);
            Win32API.PostMessage(i_PaxWindowData.p_Handle, Constants.WM_LBUTTONDOWN, 1, MakeXYParam((int)_MousePos.X, (int)_MousePos.Y)); // left click
            await Task.Delay(10);
            Win32API.PostMessage(i_PaxWindowData.p_Handle, Constants.WM_LBUTTONUP, 0, MakeXYParam((int)_MousePos.X, (int)_MousePos.Y)); // left click
            await Task.Delay(10);
        }
        
        
        private static int MakeXYParam(int _lo, int _hi)
        {
            return (int)((_hi << 16) | (_lo & 0xFFFF));
        }
    }
}
