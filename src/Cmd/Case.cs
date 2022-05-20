using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	/// <summary>
	/// Remebers what test case we are not performing (the results of later checks will be bound to this test case)
	/// </summary>
	public class Case : BaseCmd
	{
		public string Name;

		public override void ParseCmd( string[] cmd )
		{
			Name = cmd[1];
		}

		public override ExitCode Execute()
		{
			Context.Instance.Case = Name;
			return ExitCode.Success;
		}
	}
}
