using System;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TestCaser
{
	public class WebreqSpec
	{
		public string Preset;
		public string Method; // "get", "put", "post"
		public string Url; // "api/blabla?x=7"
		public JToken Body; // used for put and post methods


		public string GetMethod() => Get().Method;
		public string GetUrl() => Get().Url;
		public string GetBody()
		{
			var body = Get().Body;
			if( body.Type == JTokenType.String )
				return (string) (body as JValue).Value;
			if( body.Type == JTokenType.Object )
				return (body as JObject).ToString( Formatting.None );
			if( body.Type == JTokenType.Array )
				return (body as JArray).ToString( Formatting.None );
			throw new Exception("Invalid webreq body - not a string or json object");
		}


		public WebreqSpec Get()
		{
			if (!string.IsNullOrEmpty( Preset ))
			{
				var spec = FileTools.GetSpec<WebreqSpec>( Preset, Context.WebreqSpecsFolder );
				return spec;
			}

			if( string.IsNullOrEmpty( Url ) )
			{
				throw new Exception("Missing url in the webreqSpec");
			}

			return this;
		}

		public static WebreqSpec From( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return new WebreqSpec() { Url = jtok.Value<string>() };
			}
			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<WebreqSpec>();
			}
			throw new Exception( "Invalid webreq spec" );
		}

		public static WebreqSpec From( string txt )
		{
			if (string.IsNullOrEmpty( txt )) throw new Exception( "Empty webreq spec" );

			if (Tools.IsJsonObj( txt ))
			{
				return JsonConvert.DeserializeObject<WebreqSpec>( txt );
			}
			else
			{
				return new WebreqSpec() { Url = txt };
			}
		}
	}


}
