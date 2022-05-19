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
	/// <summary>
	/// Executes a Scriban script
	/// </summary>
	public class ScriptexEvaluator
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		static string NoRenamer( MemberInfo member ) { return member.Name; }
		static bool AnyMember( MemberInfo member ) { return true; }

		public ScriptexEvaluator()
		{
		}

		public object Evaluate( string scriptText, string sourceFilePath, object model )
		{	
			var lexerOpts = new Scriban.Parsing.LexerOptions() { Mode = Scriban.Parsing.ScriptMode.ScriptOnly };
			var template = Template.Parse( scriptText, sourceFilePath, lexerOptions: lexerOpts );
			if( template.HasErrors )
			{
				throw new Exception( $"Scriptex parse error: {string.Join("\n", template.Messages)}" );
			}

			var context = new TemplateContext();
			context.MemberRenamer = NoRenamer;
			context.MemberFilter = AnyMember;
			context.Language = Scriban.Parsing.ScriptLang.Default;
			context.TemplateLoader = new MyIncludeFromDisk();
			
			ScriptObject rootSO = new ScriptObject();
			rootSO.Import( model, renamer: NoRenamer, filter: AnyMember );
			Functions.Register( rootSO );
			context.PushGlobal( rootSO );

			return template.Evaluate( context );
		}
	}
}
