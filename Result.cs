using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace TestCaser
{
	public class Result
	{
		Context ctx = Context.Instance;

		public Result()
		{
		}

		public void Clear()
		{
			var fname = GetFileName();
			try { File.Delete( fname ); }
			catch {}
		}

		string GetFileName()
		{
			var fname = $"{Context.ResultFolder}\\{ctx.Case}.txt";
			return fname;
		}

		public void Add( string statusCode, string cmdCode, params string[] args )
		{
			Directory.CreateDirectory( Context.ResultFolder );
			var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var line = $"[{timeStamp}]:{statusCode}:{ctx.Phase}:{cmdCode}:{String.Join(':', args)}\n";
			var fname = GetFileName();
			File.AppendAllText( fname, line );
		}

		
		static void ClearOutputImages()
		{
			var files = Directory.GetFiles( Context.OutputImgFolder );
			foreach( var fname in files )
			{
				try { File.Delete( fname ); }
				catch {}
			}
		}

		static void ClearResultFiles()
		{
			var resultFiles = Directory.GetFiles( Context.ResultFolder );
			foreach( var fname in resultFiles )
			{
				try { File.Delete( fname ); }
				catch {}
			}
		}

		public static void ClearAll()
		{
			ClearResultFiles();
			ClearOutputImages();
		}



		static string GetStatus( string line )
		{
			var afterTimeStamp = line[26..];
			var segm = afterTimeStamp.Split(':');
			return segm[0];
		}


		public static bool AllPassed()
		{
			// check if there is any failure or error reported in the results
			var resultFiles = Directory.GetFiles( Context.ResultFolder );
			foreach( var fname in resultFiles )
			{
				var lines = File.ReadAllLines( fname );
				foreach( var line in lines )
				{
					var status = GetStatus( line );
					if( status == "ERROR" || status == "FAIL" )
					{
						return false;
					}
				}
			}
			return true;
		}


	}
}
