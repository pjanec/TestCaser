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
		/// <summary> command id </summary>
		public virtual string Code => GetType().Name.ToLower();

		public BaseCmd()
		{
		}

		/// <summary> fills the command data from a text command line parsed to segments; cmd[0]=cmdId, cmd[1]=1st arg etc.</summary>
		public virtual void ParseCmd( string[] cmd )
		{
			// here we get if command does not define own parse routine
			// usually because it takes no parameters
		}

		/// <summary>
		/// short string describing the command in the result so the user knows which one was it
		/// (does not include command id)
		/// </summary>
		/// <returns></returns>
		public virtual string Brief { get { return null; }}

		/// <summary> executes the command and returns exit code (0=success) </summary>
		public virtual ExitCode Execute( )
		{
			return ExitCode.Failure;
		}
	}
}
