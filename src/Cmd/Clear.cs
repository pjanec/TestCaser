using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	/// <summary>
	/// Clears all stored results.
	/// The "Case" command needs to follow to define a name for storing the results of the checks that follow.
	/// </summary>
	public class Clear : BaseCmd
	{
		public override ExitCode Execute()
		{
			try
			{
				// deletes the contents of the Results folder
				Results.ClearAll();

				// deletes the contents of the Current Status
				if( System.IO.Directory.Exists( Context.CurrentStatusFolder ) )
				{
					Tools.RecursiveDelete( new System.IO.DirectoryInfo( Context.CurrentStatusFolder ), deleteJustContent:true );
				}
				else
				{
					System.IO.Directory.CreateDirectory( Context.CurrentStatusFolder );
				}

				return ExitCode.Success;
			}
			catch
			{
				return ExitCode.Error;
			}
		}
	}
}
