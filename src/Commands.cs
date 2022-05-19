using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser
{
	/// <summary>
	/// Repository of all available commands. (see ctor for registering new commands)
	/// Can
	///   - execute command from given command line
	///   - create en empty command instance from given commandId string,
	///   - deserialize the result of the command from json into a command-specific class
	/// </summary>
	public class Commands
	{
		static Commands _inst;
		public static Commands Instance
		{
			get
			{
				if( _inst == null ) _inst = new Commands();
				return _inst;
			}
		}

		public Commands()
		{
			Register<Cmd.Clear, BaseResult>();
			Register<Cmd.Case, BaseResult>();
			Register<Cmd.Phase, BaseResult>();
			Register<Cmd.Passed, BaseResult>();
			Register<Cmd.Report, BaseResult>();
			Register<Cmd.Watchf, BaseResult>();
			Register<Cmd.Findimg, Cmd.Findimg.Result>();
			Register<Cmd.Regexf, Cmd.Regexf.Result>();
			Register<Cmd.Screenshot, Cmd.Screenshot.Result>();
			Register<Cmd.Result, Cmd.Result._Result>();
		}

		class CmdRecord
		{
			public Func<BaseCmd> CommandCreator;
			public Func<string, BaseResult> ResultDeserializer;
		}

		static Dictionary<string, CmdRecord> _commands = new Dictionary<string, CmdRecord>();

		public BaseResult DeserializeResult( string cmdCode, string jsonStr )
		{
			if( _commands.TryGetValue( cmdCode, out var rec ) )
			{
				return rec.ResultDeserializer( jsonStr );
			}
			return null;
		}

		public BaseCmd InstantiateCommand( string cmdCode )
		{
			if( _commands.TryGetValue( cmdCode, out var rec ) )
			{
				return rec.CommandCreator();
			}
			return null;
		}

		public void Register<TCmd, TResult>()
			where TCmd:BaseCmd, new()
			where TResult:BaseResult
		{
			var cmdCode = new TCmd().Code.ToLower();
			_commands[cmdCode] = new CmdRecord()
			{
				ResultDeserializer = (string json) => JsonConvert.DeserializeObject<TResult>( json ),
				CommandCreator = () => new TCmd()
			};
		}

		public ExitCode Execute( string[] cmdLine )
		{
			var cmd = InstantiateCommand( cmdLine[0].ToLower() );
			if( cmd != null )
			{
				try
				{
					cmd.ParseCmd( cmdLine );
					return cmd.Execute();
				}
				catch( Exception ex )
				{
					var br = new BaseResult()
					{
						CmdCode = cmd.Code,
						Brief = string.Join(' ', cmdLine[1..]),
						Status = EStatus.ERROR,
						StackTrace = new StackTrace( ex, true ).ToString(),
						Error = ex.Message
					};
					Results.Add( br );
					return ExitCode.Error;
				}
			}
			return ExitCode.Error;
		}

	}
}
