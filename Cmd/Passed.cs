using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	public class Passed : BaseCmd
	{
		[JsonIgnore]
		public override string Code => "passed";

		public override ExitCode Execute()
		{
			if( Results.AllPassed() )
				return ExitCode.Success;
			return ExitCode.Failure;
		}
	}
}
