using System;
using System.Drawing;
using System.IO;
using System.Linq;
using NLog;

namespace TestCaser
{
	class Program
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		static int Main( string[] args )
		{
			Context.Instance = new Context();

			if( args.Length > 0 && args[0] == "--tests" )
			{
				Test1();
				return 0;
			}

			var exitCode = ProcessCmd( args );
			return (int) exitCode;
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
			Test( new string[] {"report", "results" } ); // generates result report

		}
	}
}
