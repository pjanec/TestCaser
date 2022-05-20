using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	/// <summary>
	/// Checks if all tests executed so far passed.
	/// </summary>
	public class Passed : BaseCmd
	{
		public override ExitCode Execute()
		{
			if( Results.AllPassed() )
				return ExitCode.Success;
			return ExitCode.Failure;
		}
	}
}
