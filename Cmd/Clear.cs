using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	public class Clear : BaseCmd
	{
		[JsonIgnore]		
		public override string Code => "clear";

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
