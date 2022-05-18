using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

		public static void Add( BaseResult result )
		{
			AddLine( result.Status.ToString(), result.CmdCode, result.Brief, result );
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
			public string Brief;
			public JToken Details; // json
		}

		public static void AddLine( string statusCode, string cmdCode, string brief, BaseResult details )
		{
			Directory.CreateDirectory( Context.ResultFolder );

			var rl = new ResultLine()
			{
				TimeStamp = DateTime.Now,
				Status = statusCode,
				Phase = ctx.Phase,
				Operation = cmdCode,
				Brief = brief,
				Details = JToken.FromObject( details )
			};

			var line = JsonConvert.SerializeObject(rl, new JsonSerializerSettings() { NullValueHandling=NullValueHandling.Ignore })+"\n";
				
			var fname = GetFileName();
			File.AppendAllText( fname, line );
		}

		public static void ParseLine( string line, out ResultLine rl )
		{
			rl = JsonConvert.DeserializeObject<ResultLine>( line );
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
