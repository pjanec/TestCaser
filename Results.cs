using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;

namespace TestCaser
{
	public static class Results
	{
		static Context ctx = Context.Instance;

		public static void Clear()
		{
			var fname = GetFileName();
			try { File.Delete( fname ); }
			catch {}
		}

		static string GetFileName()
		{
			var fname = $"{Context.ResultFolder}\\{ctx.Case}.txt";
			return fname;
		}

		public static void Add( string statusCode, string cmdCode, params string[] args )
		{
			Directory.CreateDirectory( Context.ResultFolder );
			var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var line = $"[{timeStamp}]:{statusCode}:{ctx.Phase}:{cmdCode}:{String.Join(':', args)}\n";
			var fname = GetFileName();
			File.AppendAllText( fname, line );
		}

		public static void Add( BaseResult result )
		{
			Add( result.Status.ToString(), result.CmdCode, JsonConvert.SerializeObject( result, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				} ) );
		}

		public static void ClearAll()
		{
			// deletes the contents of the Results folder
			if( Directory.Exists( Context.ResultFolder ) )
			{
				Tools.RecursiveDelete( new DirectoryInfo( Context.ResultFolder ), deleteJustContent:true );
			}
		}

		public class ResultLine
		{
			public DateTime TimeStamp;
			public string Status;
			public string Phase;
			public string Operation;
			public BaseResult Details; // json
		}

		public static void ParseLine( string line, out ResultLine rl )
		{
			rl = new ResultLine();
			rl.TimeStamp = DateTime.ParseExact(
				line[1..24],
				"yyyy-MM-dd HH:mm:ss.fff",
                System.Globalization.CultureInfo.InvariantCulture);

			var afterTimeStamp = line[26..];
			var segm = afterTimeStamp.Split(':', 4);
			var num = segm.Length;
			rl.Status = num > 0 ? segm[0] : string.Empty;
			rl.Phase  = num > 1 ? segm[1] : string.Empty;
			rl.Operation   = num > 2 ? segm[2] : string.Empty;
			if( num > 3 ) rl.Details = Commands.Instance.DeserializeResult( rl.Operation, segm[3] );
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
					ParseLine( line, out var rl );
					if( rl.Status == "ERROR" || rl.Status == "FAIL" )
					{
						return false;
					}
				}
			}
			return true;
		}


	}
}
