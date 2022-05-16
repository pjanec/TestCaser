using Newtonsoft.Json;

namespace TestCaser
{
	/// <summary>
	/// Args keep all the input parameters of the command provided by the requestor
	/// </summary>
	public class BaseArgs
	{
		[JsonIgnore]
		public string[] CmdLine;
		public virtual void Parse( string[] cmd ) {}
	}
}
