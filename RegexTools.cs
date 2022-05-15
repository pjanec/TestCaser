using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TestCaser
{
	static class RegexTools
	{
		public class Spec
		{
			public string Pattern;
			public string Contains;
			public bool IgnoreCase;

			/// <summary>success if no line matching the regex</summary>
			public bool NotMatch;

			public Regex Regex {
				get
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
			}
		}

		public static Spec GetSpec( string regexId )
		{
			var fname = $"{Context.RegExFolder}\\{regexId}.json";
			var jsonStr = File.ReadAllText( fname );
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Spec>( jsonStr );
		}
		
	}
}
