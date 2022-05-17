using System;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TestCaser
{
	public class RegexSpec
	{
		public string Preset;
		public string Pattern;
		public bool IgnoreCase;


		public Regex GetRegex()
		{
			if( !string.IsNullOrEmpty(Preset) )
			{
				var fname = $"{Context.FileSpecsFolder}\\{Preset}.json";
				var jsonStr = File.ReadAllText( fname );
				var spec = JsonConvert.DeserializeObject<RegexSpec>( jsonStr );
				return spec.GetRegex();
			}

			if (!string.IsNullOrEmpty( Pattern ))
			{
				var options = IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
				var re = new Regex( Pattern, options );
				return re;
			}

			throw new Exception($"Invalid regex preset");
		}

		public static RegexSpec From( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return new RegexSpec() { Preset = jtok.Value<string>() };
			}
			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<RegexSpec>();
			}
			throw new Exception("Invalid window spec");
		}

		public static RegexSpec FromId( string txt )
		{
			if( string.IsNullOrEmpty(txt) ) throw new Exception("Empty regex locator");

			if( Tools.IsJsonObj(txt) )
			{
				return JsonConvert.DeserializeObject<RegexSpec>( txt );
			}
			else
			{
				return new RegexSpec() { Preset = txt };
			}
		}
	}

	// match info usable for scriban script
	public class RegexMatch
	{
		public string[] Groups;
	}

}
