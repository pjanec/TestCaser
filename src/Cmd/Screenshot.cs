using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{

	/// <summary>
	/// Makes a screenshot of given area and saves it as an image file
	/// </summary>
	public class Screenshot : BaseCmd
	{
		[JsonIgnore]		
		public override string Code => "screenshot";

		public string ImgId;
		public ScreenSearcher.Args Args = new ScreenSearcher.Args();

		public override string Brief => ImgId;

		public override void ParseCmd( string[] cmd )
		{
			ImgId = cmd[1];
			if( cmd.Length > 2 && Tools.IsJsonObj( cmd[2] ) ) Args = JsonConvert.DeserializeObject<ScreenSearcher.Args>( cmd[2] );
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
				var path = m.SaveImage( Args );
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
