using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser
{

	/// <summary>
	/// Base class for all commands
	/// </summary>
	public class BaseCmd
	{
		[JsonIgnore]		
		public virtual string Code => throw new NotImplementedException();

		public BaseCmd()
		{
		}

		public virtual void ParseCmd( string[] cmd )
		{
			// here we get if command does not define own parse routine
			// usually because it takes no parameters
		}


		public virtual ExitCode Execute( )
		{
			return ExitCode.Failure;
		}
	}
}
