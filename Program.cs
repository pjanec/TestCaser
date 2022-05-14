using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace TestCaser
{
	class Program
	{
		static int Main( string[] args )
		{
			Context.Instance = new Context();


			var exitCode = ProcessCmd( args );
			return (int) exitCode;

			//Test1();
			return 0;
		}

		static CmdProc.ExitCode ProcessCmd( string[] cmd )
		{
			var cmdProc = new CmdProc();
			return cmdProc.Process( cmd );
		}

		static void Test( string[] cmd )
		{
			Console.WriteLine( String.Join(" ", cmd ) );
			Console.WriteLine( ProcessCmd( cmd ) );
		}

		static void Test1()
		{
			Test( new string[] {"clear" } ); // clear all previous results
			Test( new string[] {"case", "MyTestCase1" } );
			Test( new string[] {"phase", "Phase1" } );
			Test( new string[] {"watchf", "ig", "IgLog.txt" } );
			Test( new string[] {"regex", "ig", "HelloDolly" } );
			Test( new string[] {"regex", "ig", "HelloDolly" } );
			Test( new string[] {"findimg", "pattern2.png"} );
			//Test( new string[] {"saveimg", "img2", "{'Area':{'X':10,'Y':20,'Width':100,'Height':100}}" } );
			Test( new string[] {"saveimg", "img2.jpg" } );
			Test( new string[] {"passed" } ); // checks if all test cases passed

		}
	}
}
