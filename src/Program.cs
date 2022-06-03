using System;
using System.Drawing;
using System.IO;
using System.Linq;
using NLog;
using CommandLine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TestCaser
{
	class Program
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		class Options
        {
            [Option("tests", Default=false, Required=false, HelpText = "Run internal tests.")]
            public bool RunTests { get; set; }

            [Option('d', "data", Default="", Required=false, HelpText = "Data folder.")]
            public string DataFolder { get; set; }

			[Value(0, Required=false)]
			public IEnumerable<string> Args { get; set; }
		}

		static Options opts;

		static int Main( string[] args )
		{
			bool cmdLineError = false;
			var cmdLineRes = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
					opts = o;
				} )
				.WithNotParsed(errors =>
				{
					cmdLineError = true;
				});

			if( cmdLineError )
			{
				return (int) ExitCode.Error;
			}

			if( opts.RunTests )
			{
				Test1();
				return 0;
			}

			var exitCode = ProcessCmd( opts.Args.ToArray(), opts.DataFolder );

			return (int) exitCode;
		}

		static ExitCode ProcessCmd( string[] cmd, string dataFolder )
		{
			var tc = new TC( dataFolder );
			return tc.Run( cmd );
		}

		static void Test( string[] cmd )
		{
			Console.WriteLine( String.Join(" ", cmd ) );
			Console.WriteLine( ProcessCmd( cmd, null ) );
		}

		static void Test1()
		{
			//AreaSpec.Test();

			Test( new string[] { "clear" } ); // clear all previous results
			Test( new string[] { "case", "MyTestCase1" } );
			Test( new string[] { "phase", "Phase1" } );
			//Test( new string[] { "watchf", "{newest:{path:'./*.log', recursive:true}}", "{id:'ig'}" });
			Test( new string[] { "watchf", "IgLog.txt", "{id:'ig'}" });
			//Test( new string[] { "regexf", "{watch:'ig'}", "Dolly.*" } );
			//Test( new string[] { "regexf", "IgLog2.txt", "{preset:'HelloDolly'}", "{NotMatch:true}" } );
			//Test( new string[] { "regexf", "IgLog.txt", "{pattern:'amount (\\\\d+)\\\\s*(\\\\w+)'}", "{expr:'string.to_int(Groups[1]) < 9',fromBeginning:true}" } );
			Test( new string[] { "regexf", "{watch:'ig'}", "{pattern:'HelloDolly'}", "{fromBeginning:true}" } );
			Test( new string[] { "findimg", "Images/Screenshot_35.png", "{area:{Window:{Title:'Slovn'}}}" } );
			Test( new string[] { "screenshot", "img1", "{Area:[10,20,'30%','40%']}" } );
			Test( new string[] { "screenshot", "img2.jpg", "{Area:{rect:{X:10,Y:20,Width:100,Height:100}}}" } );
			Test( new string[] { "result", "myBrief", "FAIL", "Something failed."  } ); // checks if all test cases passed
			Test( new string[] { "passed" } ); // checks if all test cases passed
			Test( new string[] { "report", "results" } ); // generates result report
			//Test( new string[] { "webreq", "https://jsonplaceholder.typicode.com", "todos/1", "{expr:'Json.userId == 1'}" } );
			//Test( new string[] { "webreq", "https://jsonplaceholder.typicode.com", "{url:'todos/1',method:'put',body:'hi!'}", "{expr:'Json.userId == 1'}" } );
			Test( new string[] { "webreq", "https://jsonplaceholder.typicode.com", "{url:'todos/1',method:'put',body:{num:3,txt:'hi!'}}", "{expr:'Json.userId == 1'}" } );

		}
	}
}
