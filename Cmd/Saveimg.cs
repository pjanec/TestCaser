using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{

	public class Saveimg : BaseCmd
	{
		[JsonIgnore]		
		public override string Code => "saveimg";

		public string ImgId;
		public ImgProc.Args Args = new ImgProc.Args();

		public override void ParseCmd( string[] cmd )
		{
			ImgId = cmd[1];
			if( cmd.Length > 2 && Tools.IsJsonObj( cmd[2] ) ) Args = JsonConvert.DeserializeObject<ImgProc.Args>( cmd[2] );
		}

		public class Result : BaseResult
		{
			public string Path;
			public Saveimg Cmd;
		}

		public override ExitCode Execute()
		{
			// get lines from the watched file
			var m = new ImgProc( ImgId );
			try
			{
				var path = m.SaveImage( Args );
				var relPath = Functions.path_getrelative( path, Context.ResultFolder );

				Results.Add( new Result() { Cmd=this, CmdCode=Code, Status=EStatus.OK, Path = relPath });
				return ExitCode.Success;
			}
			catch( Exception ex  )
			{
				Results.Add( new Result() { Cmd=this, CmdCode=Code, Status=EStatus.FAIL, Error = ex.Message });
				return ExitCode.Failure;
			}
		}
	}
}
