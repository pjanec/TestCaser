using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{

	/// <summary>
	/// Starts monitoring a file for appending new lines;
	/// the fileId is then used in the regexf command to reference the file where to search for regexps.
	/// </summary>
	public class Watchf : BaseCmd
	{
		[JsonIgnore]		
		public override string Code => "watchf";

		public string FileId;
		public string FileLocator;

		public override void ParseCmd( string[] cmd )
		{
			FileId = cmd[1];
			FileLocator = cmd[2];
		}

		public override string Brief => FileId;

		public override ExitCode Execute()
		{

			// get lines from the watched file
			var wf = FileWatcher.Create( FileId, FileLocator );
			var lines = wf.GetLines();
			wf.Save(); // remember new offset
			return ExitCode.Success;
		}
	}
}
