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

		public static void Add( BaseCmd cmd,  BaseResult result )
		{
			if( cmd != null )
			{
				result.Brief = cmd.Brief;
				result.CmdCode = cmd.Code;
				result.CmdData = JToken.FromObject(cmd);
			}
			Add( result );
		}

		public static void Add( BaseResult result )
		{
			Directory.CreateDirectory( Context.ResultFolder );

			result.TimeStamp = DateTime.Now;
			result.Phase = ctx.Phase;

			var line = JsonConvert.SerializeObject(result, new JsonSerializerSettings() { NullValueHandling=NullValueHandling.Ignore })+"\n";
				
			var fname = GetFileName();
			File.AppendAllText( fname, line );
		}

		public static void ClearAll()
		{
			// deletes the contents of the Results folder
			if( Directory.Exists( Context.ResultFolder ) )
			{
				Tools.RecursiveDelete( new DirectoryInfo( Context.ResultFolder ), deleteJustContent:true );
			}
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
					var res = JsonConvert.DeserializeObject<BaseResult>( line );
					if( res.Status == EStatus.ERROR || res.Status == EStatus.FAIL )
					{
						return false;
					}
				}
			}
			return true;
		}


	}
}
