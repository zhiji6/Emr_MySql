using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;

namespace Bifrost
{
    public class MouseHook
    {
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
        //全局的事件 
        public event MouseEventHandler OnMouseActivity;
        static int hMouseHook = 0;   //鼠标钩子句柄 
        //鼠标常量 
        public const int WH_MOUSE_LL = 14;   //mouse   hook   constant 
        HookProc MouseHookProcedure;   //声明鼠标钩子事件类型. 
        //声明一个Point的封送类型 
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }
        //声明鼠标钩子的封送结构类型 
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
        //装置钩子的函数 
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //int idHook 要安装的钩子类型 (参考下面的IdHook取值)
        //HOOKPROC lpfn 钩子过程的指针 ,也即拦截到指定系统消息后的预处理过程,须定义在DLL中
        //HINSTANCE hMod 应用程序实例的句柄 如果是全局钩子， hInstance是DLL句柄（DllMain中给的模块地址。就是包含HookProc的动态库加载地址。否则给0就可以了，即勾自己。 
        //ThreadId 要安装钩子的线程ID ,指定被监视的线程，如果明确指定了某个线程的ID就只监视该线程，此时的钩子即为线程钩子；如果该参数被设置为0，则表示此钩子为监视系统所有线程的全局钩子。);
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸下钩子的函数 
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //下一个钩挂的函数 
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        ///   <summary> 
        ///   墨认的构造函数构造当前类的实例. 
        ///   </summary> 
        public MouseHook()
        {
            //Start(); 
        }
        //析构函数. 
        ~MouseHook()
        {
            Stop();
        }
        public void Start()
        {
            //安装鼠标钩子 
            if (hMouseHook == 0)
            {
                //生成一个HookProc的实例. 
                MouseHookProcedure = new HookProc(MouseHookProc);
                Process cProcess = Process.GetCurrentProcess();
                ProcessModule cModule = cProcess.MainModule;
                IntPtr mh = GetModuleHandle(cModule.ModuleName);
                hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcedure, mh, 0);
                //如果装置失败停止钩子 
                if (hMouseHook == 0)
                {
                    Stop();
                    throw new Exception("SetWindowsHookEx failed. ");
                }
            }
        }
        public void Stop()
        {
            bool retMouse = true;
            if (hMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(hMouseHook);
                hMouseHook = 0;
            }

            //如果卸下钩子失败 
            try
            {
                if (!(retMouse)) throw new Exception("UnhookWindowsHookEx   failed. ");
            }
            catch (Exception)
            {
                
            }
        }
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //如果正常运行并且用户要监听鼠标的消息 
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseButtons button = MouseButtons.None;
                int clickCount = 0;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                }
                //从回调函数中得到鼠标的信息 
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, 0);
                OnMouseActivity(this, e);
            }
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }
    }
}
