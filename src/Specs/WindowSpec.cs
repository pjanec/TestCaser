using Newtonsoft.Json;
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
		public string Preset; // preset id
		public JToken Title; // regex
		public JToken Hwnd;

		public IntPtr GetWindow()
		{
			if( Preset != null )
			{
				var spec = FileTools.GetSpec<WindowSpec>( Preset, Context.WindowSpecsFolder );
				return spec.GetWindow();
			}

			if( Title != null )
			{
				var regexSpec = RegexSpec.From( Title );
				var re = regexSpec.GetRegex();
				var hWnd = WinTools.GetHandleByTitleRegEx( re );
				return hWnd;
			}

			if( Hwnd != null )
			{
				if( Hwnd.Type == JTokenType.Integer )
				{
					return new IntPtr( Hwnd.ToObject<int>() );
				}

				if( Hwnd.Type == JTokenType.String )
				{
					var value = (long)ulong.Parse( Hwnd.ToObject<string>(), System.Globalization.NumberStyles.HexNumber);
					return new IntPtr( value );
				}
				throw new Exception($"Invalid hwnd {Hwnd}");
			}

			throw new Exception($"Invalid window spec {JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling=NullValueHandling.Ignore })}");

		}

		public static WindowSpec From( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return new WindowSpec() { Preset=jtok.Value<string>() };
			}

			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<WindowSpec>();
			}

			throw new Exception("Invalid window spec");
		}

		public static WindowSpec From( string txt )
		{
			if( string.IsNullOrEmpty(txt) ) throw new Exception("Empty window spec");

			if( Tools.IsJsonObj(txt) )
			{
				return JsonConvert.DeserializeObject<WindowSpec>( txt );
			}
			else
			{
				return new WindowSpec() { Preset=txt };
			}
		}
	}
}
