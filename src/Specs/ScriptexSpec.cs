using System;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TestCaser
{
	public class ScriptexSpec
	{
		public string Preset;
		public JToken File;	 // fileSpec
		public string Expr;	 // scriban expression to evaluate


		public string GetExpr()
		{
			if( !string.IsNullOrEmpty(Preset) )
			{
				var spec = FileTools.GetSpec<ScriptexSpec>( Preset, Context.ScriptexSpecsFolder );
				return spec.GetExpr();
			}

			if (!string.IsNullOrEmpty( Expr ))
			{
				return Expr;
			}

			if( File != null )
			{
				var fspec = FileSpec.From( File );
				var path = fspec.GetPath();
				return System.IO.File.ReadAllText( path );
			}

			throw new Exception($"Invalid scriptex preset");
		}

		public static ScriptexSpec From( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return new ScriptexSpec() { Expr = jtok.Value<string>() };
			}
			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<ScriptexSpec>();
			}
			throw new Exception("Invalid ScriptexSpec");
		}

		public static ScriptexSpec From( string txt )
		{
			if( string.IsNullOrEmpty(txt) ) throw new Exception("Empty ScriptexSpec");

			if( Tools.IsJsonObj(txt) )
			{
				return JsonConvert.DeserializeObject<ScriptexSpec>( txt );
			}
			else
			{
				return new ScriptexSpec() { Expr = txt };
			}
		}
	}

}
