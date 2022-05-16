using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	public class Case : BaseCmd
	{
		[JsonIgnore]		
		public override string Code => "case";
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
