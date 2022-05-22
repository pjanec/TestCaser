using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestCaser.Cmd
{

	/// <summary>
	/// Makes a screenshot of given area and saves it as an image file
	/// </summary>
	public class Screenshot : BaseCmd
	{
		public string ImgId;
		public JToken Args;

		public override string Brief => ImgId;

		public override void ParseCmd( string[] cmd )
		{
			ImgId = cmd[1];
			if( cmd.Length > 2 ) Args = Tools.ToJToken( cmd[2] );
		}

		public class Result : BaseResult
		{
			public string Path;
		}

		public override ExitCode Execute()
		{
			// get lines from the watched file
			var m = new ScreenSearcher( null, ImgId );
			try
			{
				var args = Args!=null ? Args.ToObject<ScreenSearcher.Args>() : new ScreenSearcher.Args();
				var path = m.SaveImage( args );
				var relPath = Functions.path_getrelative( path, Context.ResultFolder );

				Results.Add( new Result() { Brief=Brief, CmdCode=Code, Status=EStatus.OK, Path = relPath });
				return ExitCode.Success;
			}
			catch( Exception ex  )
			{
				Results.Add( new Result() { Brief=Brief, CmdCode=Code, Status=EStatus.FAIL, Error = ex.Message });
				return ExitCode.Failure;
			}
		}
	}
}
