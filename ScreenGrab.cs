using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

using WindowsDisplayAPI;
using WindowsDisplayAPI.DisplayConfig;

namespace TestCaser
{
	public static class ScreenGrab
	{
		// warning: rect must be in physical coordinates
		public static Bitmap GrabRect( Rectangle rect )
		{
			Bitmap bitmap = new Bitmap( rect.Width, rect.Height, PixelFormat.Format32bppArgb);

			Graphics captureGraphics = Graphics.FromImage(bitmap);
			captureGraphics.CopyFromScreen( rect.Left, rect.Top, 0,0, rect.Size );

			return bitmap;
		}

		//public static Bitmap GrabMonitor( int screenId )
		//{

		//	return bitmap;
		//}

		public static Bitmap GrabAllScreens()
		{
			var rect = GetAllScreensRect();
			return GrabRect( rect );
		}

		// return physical coordinates
		public static Rectangle GetAllScreensRect()
		{
			return PathInfo.GetActivePaths()
				.Select( pi => new Rectangle( pi.Position, pi.Resolution ) )
				.Aggregate(Rectangle.Union);
		}


	}
}
