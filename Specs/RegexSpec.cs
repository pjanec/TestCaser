using System;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TestCaser
{
	public class RegexSpec
	{
		public string Pattern;
		public string Contains;
		public bool IgnoreCase;


		public Regex GetRegex()
		{
			if (!string.IsNullOrEmpty( Pattern ))
			{
				var options = IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
				var re = new Regex( Pattern, options );
				return re;
			}

			if (!string.IsNullOrEmpty( Contains ))
			{
				var pattern = ".*" + Regex.Escape(Contains) + ".*";
				var options = IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
				var re = new Regex( pattern, options );
				return re;
			}

			throw new Exception($"No Pattern not Contains specified");
		}

		public static RegexSpec FromId( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return FromId( jtok.Value<string>() );
			}
			else
			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<RegexSpec>();
			}
			throw new Exception("Invalid window spec");
		}

		public static RegexSpec FromId( string id )
		{
			if( string.IsNullOrEmpty(id) ) throw new Exception("Empty regex locator");

			if( Tools.IsJsonObj(id) )
			{
				return Newtonsoft.Json.JsonConvert.DeserializeObject<RegexSpec>( id );
			}
			else
			{
				var fname = $"{Context.RegExSpecsFolder}\\{id}.json";
				var jsonStr = File.ReadAllText( fname );
				return Newtonsoft.Json.JsonConvert.DeserializeObject<RegexSpec>( jsonStr );
			}
		}
	}

	// match info usable for scriban script
	public class RegexMatch
	{
		public string[] Groups;
	}

}
