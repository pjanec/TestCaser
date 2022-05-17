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
	/// Searches an image pattern in given area on the screen.
	/// </summary>
	public class Findimg : BaseCmd
	{
		[JsonIgnore]		
		public override string Code => "findimg";

		public string ImgId;
		public ImgProc.Args Args = new ImgProc.Args();

		public override string Brief => ImgId;

		public override void ParseCmd( string[] cmd )
		{
			ImgId = cmd[1];
			if( cmd.Length > 2 && Tools.IsJsonObj( cmd[2] ) ) Args = JsonConvert.DeserializeObject<ImgProc.Args>( cmd[2] );
		}

		public class Result : BaseResult
		{
			public string ScreenshotPath;
			public string TemplatePath;
		}

		public override ExitCode Execute()
		{
			// get lines from the watched file
			var m = new ImgProc( ImgId );
			try
			{
				if (m.Search( Args, out var grabbedImage ))
				{
					// highlight the area found
					using (var graphics = Graphics.FromImage( grabbedImage ))
					{
						var rect = m.FoundAt;
						Pen redPen = new Pen( Color.Red, 3 );
						graphics.DrawRectangle( redPen, rect );
					}

					// save the grabbed image
					var templPath = m.TemplateImageFile;
					var templRelPath = Functions.path_getrelative( templPath, Context.ResultFolder );

					var grabbedPath = m.SaveImage( grabbedImage );
					var grabbedRelPath = Functions.path_getrelative( grabbedPath, Context.ResultFolder );

					Results.Add( new Result() { Brief=Brief, CmdCode=Code, Status=EStatus.OK, 
						ScreenshotPath = grabbedRelPath,
						TemplatePath = templRelPath
					});

					return ExitCode.Success;
				}

				// save the image that was grabbed
				if (!Args.NoSave)
				{
					var templPath = m.TemplateImageFile;
					var templRelPath = Functions.path_getrelative( templPath, Context.ResultFolder );

					var grabbedPath = m.SaveImage( grabbedImage );
					var grabbedRelPath = Functions.path_getrelative( grabbedPath, Context.ResultFolder );

					Results.Add( new Result() { Brief=Brief, CmdCode=Code, Status=EStatus.FAIL,
						ScreenshotPath = grabbedRelPath,
						TemplatePath = templRelPath
					});
				}
				else
				{
					Results.Add( new Result() { Brief=Brief, CmdCode=Code, Status= EStatus.FAIL });
				}
				return ExitCode.Failure;
			}
			catch (Exception ex)
			{
				Results.Add( new Result() { Brief=Brief, CmdCode=Code, Status=EStatus.FAIL, Error = ex.Message });
				return ExitCode.Failure;
			}
		}
	}
}
