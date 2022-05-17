﻿using System;
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
			Context.Instance = new Context();

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
			string origCwd = Directory.GetCurrentDirectory();

			if( !string.IsNullOrEmpty( dataFolder) )
			{
				Directory.SetCurrentDirectory( dataFolder );
			}

			var exitCode = Commands.Instance.Execute( cmd );

			Directory.SetCurrentDirectory( origCwd );

			return exitCode;
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
			Test( new string[] { "watchf", "ig", "{newest:{folder:'./*.log', recursive:true}}" } );
			Test( new string[] { "regexf", "ig", "HelloDolly", "{NotMatch:true}" } );
			Test( new string[] { "regexf", "ig", "HelloDolly" } );
			Test( new string[] { "findimg", "Screenshot_35.png", "{WinTitle:{RegExId:'Slovn'}}" } );
			Test( new string[] { "saveimg", "img1", "{Area:{X:10,Y:20,Width:100,Height:100}}" } );
			Test( new string[] { "saveimg", "img2.jpg" } );
			Test( new string[] { "result", "myBrief", "FAIL", "Somethinf failed."  } ); // checks if all test cases passed
			Test( new string[] { "passed" } ); // checks if all test cases passed
			Test( new string[] { "report", "results" } ); // generates result report

		}
	}
}
