using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestCaser.Cmd
{

	/// <summary>
	/// Searches a file for given regular expression, either from beginning or
	/// just the lines appended since the last query.
	/// </summary>
	public class Regexf : BaseCmd
	{
		public JToken FileSpec;
		public JToken RegexSpec;
		public bool NotMatch;
		public bool FromBeginning;
		public JToken Expr; // expression to evaluate the status (applied only if regex returns any match)

		public override string Brief => $"{FileSpec} {RegexSpec}  {Expr}";

		public class ScriptexDataModel
		{
			public List<string> Groups = new List<string>();

			public ScriptexDataModel()
			{
			}

			public ScriptexDataModel( System.Text.RegularExpressions.Match match )
			{
				foreach( var g in match.Groups )
				{
					Groups.Add( g.ToString() );
				}
			}
		}

		public class Result : BaseResult
		{					
			public string Expr;	// the expression evaluated
			public ScriptexDataModel Match; // the model for that expression
		}

		public override void ParseCmd( string[] cmd )
		{
			FileSpec = Tools.ToJToken( cmd[1] );
			RegexSpec = Tools.ToJToken( cmd[2] );
			if( cmd.Length > 3 && Tools.IsJsonObj( cmd[3] ) ) JsonConvert.PopulateObject( cmd[3], this );
		}


		public override ExitCode Execute()
		{
			var fspec = TestCaser.FileSpec.From( FileSpec );

			// get lines from the watched file
			var wf = new FileWatcher( fspec.Watch, fspec.GetPath(), FileWatcher.Mode.LoadOrCreate );
			var lines = wf.GetLines( FromBeginning );
			wf.Save(); // remember new offset

			// apply regex
			try
			{
				var args = new FileRegexSearcher.Args()
				{
					NotMatch = NotMatch
				};
				
				var regexSpec = TestCaser.RegexSpec.From(RegexSpec );
				var re = new FileRegexSearcher( regexSpec.GetRegex(), args );
				bool success = re.Search( lines, out var match );
				if( !success )
				{	
					// log the result
					Results.Add( this, new Result() { Status=EStatus.FAIL, Error = "No match" });
					return ExitCode.Failure;
				}

				// apply expression on the match
				if( Expr != null )
				{
					var exprSpec = ScriptexSpec.From( Expr );
					var exprStr = exprSpec.GetExpr();
					var model = new ScriptexDataModel( match );
					var evaluator = new ScriptexEvaluator();
					object result;
					try
					{
						result = evaluator.Evaluate( exprStr, null, model );
					}
					catch( Exception ex )
					{
						Results.Add( this, new Result()
						{
							Status=EStatus.ERROR,
							Error = ex.Message,
							Expr = exprStr,
							Match = model
						});
						return ExitCode.Error;
					}

					if( result is bool )
					{
						if( !((bool)result) ) // not passed
						{
							Results.Add( this, new Result()
							{
								Status=EStatus.FAIL, Error="Expression returned false",
								Expr = exprStr,
								Match = model
							});
							return ExitCode.Failure;
						}
						else
						{
							Results.Add( this, new Result()
							{
								Status=EStatus.OK,
								Expr = exprStr,
								Match = model
							});
							return ExitCode.Success;
						}
					}
					else
					{
						Results.Add( this, new Result()
						{
							Status=EStatus.ERROR, Error="Expression did not return a bool value",
							Expr = exprStr,
							Match = model
						});
						return ExitCode.Error;
					}
				}
				else
				{
					Results.Add( this, new Result()
					{
						Status=EStatus.OK,
						Match = new ScriptexDataModel( match )
					});
					return ExitCode.Success;
				}
			}
			catch(Exception ex)
			{
				Results.Add( this, new Result() { Status = EStatus.ERROR, Error = ex.Message } );

				return ExitCode.Failure;
			}
		}
	}

}
