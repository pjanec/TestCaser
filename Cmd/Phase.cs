using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
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
