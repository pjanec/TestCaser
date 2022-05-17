using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	/// <summary>
	/// Remebers what test case phase we are not performing
	/// (shown in the report to help identify in what stage the test case was)
	/// </summary>
	public class Phase : BaseCmd
	{
		[JsonIgnore]
		public override string Code => "phase";
		public string Name;

		public override void ParseCmd( string[] cmd )
		{
			Name = cmd[1];
		}

		public override ExitCode Execute()
		{
			Context.Instance.Phase = Name;
			return ExitCode.Success;
		}
	}
}
