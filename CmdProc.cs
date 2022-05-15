using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLog;
using System.Drawing;

namespace TestCaser
{
	public class CmdProc
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		Context _ctx = Context.Instance;

		public ExitCode Process( string[] cmd )
		{
			// cmd[0] = command name
			// cmd[1] = arg 1
			// ...
			var exitCode = ExitCode.Error;
			var cmdName = cmd[0].ToLower();
			if( cmdName == "case" )
			{
				exitCode = HandleCase( cmd );
			}
			else
			if( cmdName == "phase" )
			{
				exitCode = HandlePhase( cmd );
			}
			else
			if( cmdName == "watchf" )
			{
				exitCode = HandleWatchf( cmd );
			}
			else
			if( cmdName == "regexf" )
			{
				exitCode = HandleRegexf( cmd );
			}
			else
			if( cmdName == "findimg" )
			{
				exitCode = HandleFindImg( cmd );
			}
			else
			if( cmdName == "saveimg" )
			{
				exitCode = HandleSaveImg( cmd );
			}
			else
			if( cmdName == "clear" )
			{
				exitCode = HandleClear( cmd );
			}
			else
			if( cmdName == "passed" )
			{
				exitCode = HandlePassed( cmd );
			}
			else
			if( cmdName == "report" )
			{
				exitCode = HandleReport( cmd );
			}
			return exitCode;
		}

		ExitCode HandleCase( string[] cmd )
		{
			_ctx.Case = cmd[1];

			//// clears the result for this test case
			//new Result().Clear();
			// in case we run the same test case multiple times, we want all the results...

			return ExitCode.Success;
		}

		ExitCode HandlePhase( string[] cmd )
		{
			_ctx.Phase = cmd[1];
			return ExitCode.Success;
		}

		ExitCode  HandleWatchf( string[] cmd )
		{
			var fileId = cmd[1];
			var fileLocator = cmd[2];
			var wf = new Watchf( fileId, fileLocator );
			wf.Save();
			return ExitCode.Success;
		}

		ExitCode HandleRegexf( string[] cmd )
		{
			var fileId = cmd[1];
			var regexId = cmd[2];
			var jsonArgs =  cmd.Length > 3 ? cmd[3] : string.Empty;

			var args = string.IsNullOrEmpty( jsonArgs )
				?  new FileRegEx.Args()
				: Newtonsoft.Json.JsonConvert.DeserializeObject<FileRegEx.Args>( jsonArgs );

			// get lines from the watched file
			var wf = new Watchf( fileId );
			var lines = wf.GetLines();
			wf.Save(); // remember new offset

			// apply regex
			try
			{
				var re = new FileRegEx( regexId, args );
				bool success = re.Search( lines );
				if( !success )
				{	
					// log the result
					var res = new Result();
					res.Add( "FAIL", "regexf", regexId );
					return ExitCode.Failure;
				}
			}
			catch(Exception ex)
			{
				var res = new Result();
				res.Add( "ERROR", "regexf", regexId, ex.Message );
			}

			return ExitCode.Success;
		}

		ExitCode HandleFindImg( string[] cmd )
		{
			var imgId = cmd[1];
			var jsonArgs =  cmd.Length > 2 ? cmd[2] : string.Empty;

			var args = string.IsNullOrEmpty( jsonArgs )
				?  new ImgProc.Args()
				: Newtonsoft.Json.JsonConvert.DeserializeObject<ImgProc.Args>( jsonArgs );

			// get lines from the watched file
			var m = new ImgProc( imgId );
			try
			{
				if (m.Search( args, out var grabbedImage ))
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
					var res = new Result();
					res.Add( "OK", "findimg", imgId, grabbedRelPath, templRelPath );

					return ExitCode.Success;
				}

				// save the image that was grabbed
				if (!args.NoSave)
				{
					var templPath = m.TemplateImageFile;
					var templRelPath = Functions.path_getrelative( templPath, Context.ResultFolder );

					var grabbedPath = m.SaveImage( grabbedImage );
					var grabbedRelPath = Functions.path_getrelative( grabbedPath, Context.ResultFolder );
					var res = new Result();
					res.Add( "FAIL", "findimg", imgId, grabbedRelPath, templRelPath );
				}
				else
				{
					var res = new Result();
					res.Add( "FAIL", "findimg", imgId, "", "" );
				}
				return ExitCode.Failure;
			}
			catch (Exception ex)
			{
				var res = new Result();
				res.Add( "FAIL", "findimg", imgId, "", "", ex.Message );
				return ExitCode.Failure;
			}

		}


		ExitCode HandleSaveImg( string[] cmd )
		{
			var imgId = cmd[1];	// base filename of the image
			var jsonArgs =  cmd.Length > 2 ? cmd[2] : string.Empty;

			var args = string.IsNullOrEmpty( jsonArgs )
				?  new ImgProc.Args()
				: Newtonsoft.Json.JsonConvert.DeserializeObject<ImgProc.Args>( jsonArgs );

			// get lines from the watched file
			var m = new ImgProc( imgId );
			try
			{
				var path = m.SaveImage( args );
				var relPath = Functions.path_getrelative( path, Context.ResultFolder );
				var res = new Result();
				res.Add( "OK", "saveimg", imgId, relPath );
				return ExitCode.Success;
			}
			catch( Exception ex  )
			{
				var res = new Result();
				res.Add( "ERROR", "saveimg", imgId, "", ex.Message );
				return ExitCode.Failure;
			}
		}

		ExitCode HandleClear( string[] cmd )
		{
			try
			{
				Result.ClearAll();
				return ExitCode.Success;
			}
			catch
			{
				return ExitCode.Error;
			}
		}

		ExitCode HandlePassed( string[] cmd )
		{
			if( Result.AllPassed() )
				return ExitCode.Success;
			return ExitCode.Failure;
		}

		ExitCode HandleReport( string[] cmd )
		{
			if( cmd[1] == "results" )
			{
				try
				{
					var model = new Models.Results.ModelLoader().Load();
					new TemplateProcessor(model).ProcessFile(
						Context.TemplatesFolder+"\\Results.scriban",
						Context.ResultFolder + "\\Results.html"
					);
					return ExitCode.Success;
				}
				catch( Exception ex )
				{
					log.Error(ex.Message);
					Console.WriteLine(ex.Message);
					return ExitCode.Failure;
				}
			}
			log.Error($"Unknown report type: {cmd[1]}");
			return ExitCode.Failure;
		}

	}
}
