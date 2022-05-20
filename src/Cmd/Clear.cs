using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	/// <summary>
	/// Clears all stored results.
	/// The "Case" command needs to follow to define a name for storing the results of the checks that follow.
	/// </summary>
	public class Clear : BaseCmd
	{
		public override ExitCode Execute()
		{
			try
			{
				Results.ClearAll();
				return ExitCode.Success;
			}
			catch
			{
				return ExitCode.Error;
			}
		}
	}
}
