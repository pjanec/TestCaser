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
			//Register<Cmd.Clear>();
			//Register<Cmd.Case>();
			//Register<Cmd.Phase>();
			//Register<Cmd.Passed>();
			//Register<Cmd.Report>();
			//Register<Cmd.Watchf>();
			//Register<Cmd.Findimg>();
			//Register<Cmd.Regexf>();
			//Register<Cmd.Screenshot>();
			//Register<Cmd.Result>();
		}

		class CmdRecord
		{
			public Func<BaseCmd> CommandCreator;
		}

		static Dictionary<string, CmdRecord> _commands = new Dictionary<string, CmdRecord>();

		public BaseCmd InstantiateCommand( string cmdCode )
		{
			// try registered commands first
			if (_commands.TryGetValue( cmdCode, out var rec ))
			{
				return rec.CommandCreator();
			}

			// try to find the comamnd class via reflection
			var thisTypeName = this.GetType().FullName;
			var lastDotIndex = thisTypeName.LastIndexOf('.');
			var namespc = thisTypeName.Substring( 0, lastDotIndex );

			var typeName = namespc+".Cmd."+cmdCode.Substring(0, 1).ToUpper()+cmdCode.Substring(1).ToLower();
			var type = Type.GetType( typeName );
			if( type != null )
			{
				return (BaseCmd) Activator.CreateInstance( type );
			}

			return null;
		}

		public void Register<TCmd>()
			where TCmd:BaseCmd, new()
		{
			var cmdCode = new TCmd().Code.ToLower();
			_commands[cmdCode] = new CmdRecord()
			{
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
			else
			{
				var br = new BaseResult()
				{
					CmdCode = cmdLine[0],
					Brief = string.Join(' ', cmdLine[1..]),
					Status = EStatus.ERROR,
					Error = "Invalid command",
				};
				Results.Add( br );
			}
			return ExitCode.Error;
		}

	}
}
