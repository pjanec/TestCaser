using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestCaser
{
	public class TC
	{
		string _dataFolder;

		public TC( string dataFolder )
		{
			Context.Instance = new Context();

			//Console.WriteLine($"Data folder: {dataFolder}");
			//Console.ReadLine();

			_dataFolder = dataFolder;
		}

		public ExitCode Run( params string[] args )
		{
			//Console.WriteLine("DEBUG: "+ string.Join(", ", args));
			string origCwd = Directory.GetCurrentDirectory();

			if( !string.IsNullOrEmpty( _dataFolder) )
			{
				Directory.SetCurrentDirectory( _dataFolder );
			}

			var exitCode = Commands.Instance.Execute( args );

			Directory.SetCurrentDirectory( origCwd );

			return exitCode;
		}
	}
}
