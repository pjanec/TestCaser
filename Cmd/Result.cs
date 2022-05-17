using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{

	/// <summary>
	/// Stores user-defined result record into the report.
	/// To be used if the pass/fail state is determined by another tool and we just want to keep the record of it.
	/// </summary>
	public class Result : BaseCmd
	{
		[JsonIgnore]		
		public override string Code => "result";

		public EStatus Status;
		public string _Brief;
		public string Error;

		public override string Brief => _Brief;

		public class _Result : BaseResult
		{
			public RegexMatch Match;
		}

		public override void ParseCmd( string[] cmd )
		{
			_Brief = cmd.Length > 1 ? cmd[1] : null;

			if( Enum.TryParse( cmd[2], out Status ) ) {}
			else
			if( cmd[2].ToLower().StartsWith("fail") ) Status=EStatus.FAIL;
			else
			if( cmd[2].ToLower().StartsWith("err") ) Status=EStatus.ERROR;
			else
				Status=EStatus.OK;
		
			Error = cmd.Length > 3 ? cmd[3] : null;
		}


		public override ExitCode Execute()
		{

			Results.Add( new _Result() { CmdCode=Code, Brief=Brief, Status=Status, Error=Error });
			return ExitCode.Failure;
		}
	}
}
