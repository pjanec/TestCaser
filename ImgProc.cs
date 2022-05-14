﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace TestCaser
{
	public class ImgProc
	{
		Context ctx = Context.Instance;

		public class Area
		{
			public int X;
			public int Y;
			public int Width;
			public int Height;
		}

		public class Args
		{
			public string Method;
			public Area Area;
			public double Precision = 0.8; // threashold for patter matching; the closer to 1.0 the more exact match, if negative the default around 0.8 will be used
		}


		/// <summary>
		/// base file name with extension (img1.png, myImage2.jpg etc.)
		/// </summary>
		string _imgId;

		/// <param name="imgId">base file name with extension (img1.png, myImage2.jpg etc.)</param>
		public ImgProc( string imgId )
		{
			_imgId = imgId;
			//_imgFileName = GetPatternImgFileName( imgId );
		}

		string GetPatternImgFileName()
		{
			return $"{Context.PatternImgFolder}\\{_imgId}";
		}

		string GetOutputImgFileName()
		{
			// add extension if missing
			if( String.IsNullOrEmpty( Path.GetExtension( _imgId ) ) )
				_imgId += ".jpg";

			return $"{Context.OutputImgFolder}\\{ctx.Case}-{ctx.Phase}-{_imgId}";
		}

		public bool Search( Args args )
		{
			Rectangle rect = args.Area != null
				? new Rectangle( args.Area.X, args.Area.Y, args.Area.Width, args.Area.Height )
				: ScreenGrab.GetAllScreensRect();

			var bitmap = ScreenGrab.GrabRect( rect );
			var tmplFname = GetPatternImgFileName();
			return FindImage( bitmap, tmplFname, args.Precision, out var match );
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

		Rectangle GetAreaRect( Args args )
		{
			return args.Area != null
				? new Rectangle( args.Area.X, args.Area.Y, args.Area.Width, args.Area.Height )
				: ScreenGrab.GetAllScreensRect();
		}

		Bitmap GrabScreen( Args args )
		{
			var rect = GetAreaRect( args );
			return ScreenGrab.GrabRect( rect );
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