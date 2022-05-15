using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Drawing;

namespace TestCaser
{
    class WinTools
    {

        [DllImport("USER32.DLL")]
        static extern IntPtr GetShellWindow();

        [DllImport("USER32.DLL")]
        static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError=true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);        

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }
        delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        public static IntPtr GetHandleByTitleRegEx( Regex re )
        {
            const int nChars = 256;

            IntPtr shellWindow = GetShellWindow();
            IntPtr found = IntPtr.Zero;

            EnumWindows( delegate (IntPtr hWnd, int lParam)
            {
                //ignore shell window
                if (hWnd == shellWindow) return true;

                //get Window Title
                StringBuilder Buff = new StringBuilder(nChars);

                if (GetWindowText(hWnd, Buff, nChars) > 0)
                {
                    //Case insensitive match
                    var title = Buff.ToString();
                    if( re.IsMatch( title ) )
                    {
                        found = hWnd;
                        return true;
                    }
                }
                return true;
            }, 0 );

            return found;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LogicalToPhysicalPointForPerMonitorDPI(IntPtr hwnd, ref POINT lpPoint);

        public static bool GetWindowClientRectPhysical( IntPtr hWnd, out Rectangle physicalRect )
        {
            RECT rct;
            physicalRect = new Rectangle();
            if(!GetClientRect( hWnd, out rct )) return false;

            POINT topLeft = new POINT() { X=rct.Left, Y=rct.Top };
            if( !ClientToScreen( hWnd, ref topLeft ) ) return false;
            if( !LogicalToPhysicalPointForPerMonitorDPI( hWnd, ref topLeft ) ) return false;

            POINT bottomRight = new POINT() { X=rct.Right, Y=rct.Bottom };
            if( !ClientToScreen( hWnd, ref bottomRight ) ) return false;
            if( !LogicalToPhysicalPointForPerMonitorDPI( hWnd, ref bottomRight ) ) return false;

            physicalRect.X = topLeft.X;
            physicalRect.Y = topLeft.Y;
            physicalRect.Width = bottomRight.X - topLeft.X;
            physicalRect.Height = bottomRight.Y - topLeft.Y;
            
            return true;
        }
	}
}
