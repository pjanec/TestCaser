using System;
using System.IO;
using System.Reflection;
using System.Text;
using Scriban;
using Scriban.Runtime;
using System.Text.RegularExpressions;
using NLog;
using System.Collections.Generic;

namespace TestCaser
{
	/// <summary>
	/// Renders an output file from a template and a data model.
	/// Template is a Scriban file while the model is whatever C# object.
	/// </summary>	
	public class TemplateProcessor
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		static string NoRenamer( MemberInfo member ) { return member.Name; }
		static bool AnyMember( MemberInfo member ) { return true; }

		object _model;

		public TemplateProcessor( object model )
		{
			_model = model;
		}

		public void ProcessFile( string templFN, string outFN )
		{	
			log.Debug($"Rendering '{templFN}' => '{outFN}'");
			var templStr = System.IO.File.ReadAllText( templFN, Encoding.UTF8 );

			var lexerOpts = new Scriban.Parsing.LexerOptions() { Mode = Scriban.Parsing.ScriptMode.Default };
			var template = Template.Parse( templStr, templFN, lexerOptions: lexerOpts );
			if( template.HasErrors )
			{
				foreach( var msg in template.Messages )
				{
					log.Error( msg );
					throw new Scriban.Syntax.ScriptParserRuntimeException( msg.Span, msg.Message, new LogMessageBag() );
				}
			}


			var context = new TemplateContext();
			context.MemberRenamer = NoRenamer;
			context.MemberFilter = AnyMember;
			//context.Language = Scriban.Parsing.ScriptLang.Scientific;
			context.TemplateLoader = new MyIncludeFromDisk();
			
			ScriptObject scriptObject = new ScriptObject();
			scriptObject.Import( _model, renamer: NoRenamer, filter: AnyMember );
			Functions.Register( scriptObject );
			scriptObject["args"] = new List<object>() { System.IO.Path.GetFullPath( templFN ) };
			scriptObject["thisFile"] = System.IO.Path.GetFullPath( templFN );

			context.PushGlobal( scriptObject );

			try
			{
				var result = template.Render( context );

				// make sure the output folder exists
				var outDir = Path.GetDirectoryName( outFN );
				if( !String.IsNullOrEmpty( outDir ) ) Directory.CreateDirectory( outDir );

				// write the output file there
				System.IO.File.WriteAllText( outFN, result, Encoding.UTF8 );
			}
			catch( Scriban.Syntax.ScriptRuntimeException ex )
			{
				log.Error( ex.Message );
				throw;
			}

		}

		// file format: on each line: "template file", "output file"
		//    "Entities.tmpl", "Entities.cs"
		//    "Descriptors.tmpl", "Descriptors.cs"
		public void ProcessFileList( string fileListFileName )
		{
			var reQuotedString = new Regex(@"([""'])(?:\\.|[^\\])*?\1");
			foreach( var line in System.IO.File.ReadAllLines( fileListFileName ) )
			{
				var matches = reQuotedString.Matches(line);
				if( matches.Count < 2 ) continue;
				var templFN = matches[0].Value.Trim('"');
				var outFN = matches[1].Value.Trim('"');
				if( !string.IsNullOrEmpty(templFN) && !string.IsNullOrEmpty(outFN) )
				{
					ProcessFile( templFN, outFN );
				}
			}
		}
	}
}
