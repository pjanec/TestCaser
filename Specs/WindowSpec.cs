using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace TestCaser
{
	#pragma warning disable CS0649

	/// <summary>
	/// Specifies the coordinates of the screen area to be used by other parts
	/// </summary>
	public class WindowSpec
	{
		public ByTitleSpec ByTitle;

		public class ByTitleSpec
		{
			public JToken Regex;
		}

		public IntPtr GetWindow()
		{
			if( ByTitle != null )
			{
				var regexSpec = RegexSpec.FromId( ByTitle.Regex );
				var re = regexSpec.GetRegex();
				var hWnd = WinTools.GetHandleByTitleRegEx( re );
				return hWnd;
			}
			throw new Exception("Invalid window spec");

		}

		public static WindowSpec FromId( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return FromId( jtok.Value<string>() );
			}
			else
			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<WindowSpec>();
			}
			throw new Exception("Invalid window spec");
		}

		public static WindowSpec FromId( string id )
		{
			if( string.IsNullOrEmpty(id) ) throw new Exception("Empty window locator");

			if( Tools.IsJsonObj(id) )
			{
				return Newtonsoft.Json.JsonConvert.DeserializeObject<WindowSpec>( id );
			}
			else
			{
				var fname = $"{Context.WindowSpecsFolder}\\{id}.json";
				var jsonStr = File.ReadAllText( fname );
				return Newtonsoft.Json.JsonConvert.DeserializeObject<WindowSpec>( jsonStr );
			}
		}
	}
}
