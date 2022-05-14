using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestCaser
{
	public class CmdProc
	{
		Context _ctx = Context.Instance;

		public enum ExitCode
		{
			Success = 0,
			Failure = 1,
			Error = 2
		}


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
			if( cmdName == "regex" )
			{
				exitCode = HandleRegex( cmd );
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

		ExitCode HandleRegex( string[] cmd )
		{
			var fileId = cmd[1];
			var regexId = cmd[2];

			// get lines from the watched file
			var wf = new Watchf( fileId );
			var lines = wf.GetLines();
			wf.Save(); // remember new offset

			// apply regex
			try
			{
				var re = new RegEx( regexId );
				bool success = re.Search( lines );
				if( !success )
				{	
					// log the result
					var res = new Result();
					res.Add( "FAIL", "regex", regexId );
					return ExitCode.Failure;
				}
			}
			catch(Exception ex)
			{
				var res = new Result();
				res.Add( "ERROR", "regex", regexId, ex.Message );
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
			if (m.Search( args ))
			{
				return ExitCode.Success;
			}
			return ExitCode.Failure;
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

				var res = new Result();
				res.Add( "IMG", "saveimg", imgId, path );
				return ExitCode.Success;
			}
			catch( Exception ex  )
			{
				var res = new Result();
				res.Add( "ERROR", "saveimg", imgId, ex.Message );
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
	}
}
