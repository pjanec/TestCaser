using System;
using System.Reflection;
using System.Linq;
using System.Text;
using Scriban;
using Scriban.Runtime;
using System.Text.RegularExpressions;
using NLog;
using System.Collections.Generic;

namespace TestCaser
{
	public class ScriptProcessor
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		static string NoRenamer( MemberInfo member ) { return member.Name; }
		static bool AnyMember( MemberInfo member ) { return true; }

		public ScriptProcessor()
		{
		}

		// args[0] = script file name itself
		// args[1] = first script argument
		public int RunFile( string scriptFN, List<object> args )
		{	
			log.Debug($"Loading script '{scriptFN}'");
			var templStr = System.IO.File.ReadAllText( scriptFN, Encoding.UTF8 );
			return RunString( templStr, scriptFN, args );
		}

		public int RunString( string scriptText, string sourceFilePath, List<object> args )
		{	
			var lexerOpts = new Scriban.Parsing.LexerOptions() { Mode = Scriban.Parsing.ScriptMode.ScriptOnly };
			var template = Template.Parse( scriptText, sourceFilePath, lexerOptions: lexerOpts );
			if( template.HasErrors )
			{
				foreach( var msg in template.Messages )
				{
					log.Error( msg );
				}
				return -1;	
			}


			var context = new TemplateContext();
			context.MemberRenamer = NoRenamer;
			context.MemberFilter = AnyMember;
			context.Language = Scriban.Parsing.ScriptLang.Default;
			context.TemplateLoader = new MyIncludeFromDisk();
			
			ScriptObject rootSO = new ScriptObject();
			Functions.Register( rootSO );
			rootSO["args"] = args;
			//scriptObject["thisFile"] = System.IO.Path.GetFullPath( sourceFilePath );

			// runs TestCaser command; use: tc ['regex', 'myRE1']
			rootSO.Import("tc", new Func<IEnumerable<string>, int>((args) => (int) Commands.Instance.Execute( args.ToArray())));

			// template functions
			{
				var so = Functions.CreateNamespace(rootSO, "templ");
				so.Import("render", new Func<string, string, object, bool>((string __templFN, string __outFN, object __model) => { new TemplateProcessor(__model).ProcessFile( __templFN, __outFN); return true; } ));
			}

			// model loaders
			{
				var so = Functions.CreateNamespace(rootSO, "mdl");
				so.Import("results", new Func<object>(() => new Models.Results.ModelLoader().Load()));
			}

			context.PushGlobal( rootSO );

			try
			{
				var result = template.Evaluate( context );
			}
			catch( Scriban.Syntax.ScriptRuntimeException ex )
			{
				log.Error( ex.Message );
			}

			log.Debug($"Done.");
			return 0;
		}
	}
}
