using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Newtonsoft.Json.Linq;

namespace TestCaser
{
	public class ScreenSearcher
	{
		Context ctx = Context.Instance;

		public class Args
		{
			public string Method;
			public JToken Area;
			public double Precision = 0.8; // threashold for patter matching; the closer to 1.0 the more exact match, if negative the default around 0.8 will be used
			public bool NoSave; // do not save image if not found (used by command processor)
		}


		/// <summary>
		/// id to add to the image path and to the report to identify it
		/// </summary>
		string _imgId;

		string _tmplFname;

		Rectangle _foundAt;

		/// <summary>
		/// only after calling Search to get the path to the image searched
		/// </summary>
		public string TemplateImageFile => _tmplFname;

		public Rectangle FoundAt => _foundAt;

		/// <param name="imgId">base file name with extension (img1.png, myImage2.jpg etc.)</param>
		public ScreenSearcher( string imgTemplFilePath, string id=null )
		{
			if( string.IsNullOrEmpty(id) )
			{
				id = Guid.NewGuid().ToString();
			}

			_imgId = id;
			_tmplFname = imgTemplFilePath;
		}

		string GetOutputImgFileName()
		{
			// add extension if missing
			if( String.IsNullOrEmpty( Path.GetExtension( _imgId ) ) )
				_imgId += ".jpg";

			return $"{Context.OutputImgFolder}\\{ctx.Case}-{ctx.Phase}-{_imgId}";
		}

		public bool Search( Args args, out Bitmap grabbedImage )
		{
			Rectangle rect = GetAreaRect( args );

			var bitmap = GrabScreenRect( rect );
			grabbedImage = bitmap;
			return FindImage( bitmap, _tmplFname, args.Precision, out _foundAt );
		}

		bool FindImage( Bitmap bitmap, string templateImgFile, double threshold, out Rectangle match )
		{
			Image<Bgr, byte> source = bitmap.ToImage<Bgr, byte>();
			Image<Bgr, byte> template = new Image<Bgr, byte>(templateImgFile); // Image A

			using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
			{
				double[] minValues, maxValues;
				Point[] minLocations, maxLocations;
				result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

				// You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
				if (maxValues[0] > threshold)
				{
					// This is a match. Do something with it, for example draw a rectangle around it.
					match = new Rectangle(maxLocations[0], template.Size);
					//imageToShow.Draw(match, new Bgr(Color.Red), 3);
					return true;
				}
			}
			match = new Rectangle();
			return false;
		}

		static Rectangle GetAreaRect( Args args )
		{
			if( args.Area != null )
			{
				var areaSpec = AreaSpec.From( args.Area );
				return areaSpec.GetRect();
			}

			return AreaSpec.GetAllScreensRect();
		}


		static Bitmap GrabScreen( Args args )
		{
			var rect = GetAreaRect( args );
			return GrabScreenRect( rect );
		}

		static Bitmap GrabScreenRect( Rectangle rect )
		{
			Bitmap bitmap = new Bitmap( rect.Width, rect.Height, PixelFormat.Format32bppArgb);

			Graphics captureGraphics = Graphics.FromImage(bitmap);
			captureGraphics.CopyFromScreen( rect.Left, rect.Top, 0,0, rect.Size );

			return bitmap;
		}

		public string SaveImage( Args args )
		{
			var bitmap = GrabScreen( args );
			return SaveImage( bitmap );
		}

		public string SaveImage( Bitmap bitmap )
		{
			Directory.CreateDirectory( Context.OutputImgFolder );

			var fname = GetOutputImgFileName();
			var ext = Path.GetExtension( fname );
			ImageFormat imgFmt = ImageFormat.Png;
			if( ext == ".jpg" ) imgFmt = ImageFormat.Jpeg;
			bitmap.Save( fname, imgFmt );
			return fname;
		}
	}
}
