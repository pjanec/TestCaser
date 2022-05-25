using System;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TestCaser
{
	//public class Addr
	//{
	//	public string Protocol;	 // http
	//	public string Server;	 // 10.1.2.76
	//	public int Port;		 // 80
	//}

	public class AddrSpec
	{
		public string Preset;
		public string BaseAddress; // http://10.1.2.76:80

		public string GetBaseAddress()
		{
			if( !string.IsNullOrEmpty(Preset) )
			{
				var spec = FileTools.GetSpec<AddrSpec>( Preset, Context.AddrSpecsFolder );
				return spec.GetBaseAddress();
			}

			if( !string.IsNullOrEmpty(BaseAddress) )
			{
				return BaseAddress;
			}

			throw new Exception($"Invalid addr spec");
		}

		public static AddrSpec From( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return new AddrSpec() { BaseAddress = jtok.Value<string>() };
			}
			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<AddrSpec>();
			}
			throw new Exception("Invalid addr spec");
		}

		public static AddrSpec From( string txt )
		{
			if( string.IsNullOrEmpty(txt) ) throw new Exception("Empty addr spec");

			if( Tools.IsJsonObj(txt) )
			{
				return JsonConvert.DeserializeObject<AddrSpec>( txt );
			}
			else
			{
				return new AddrSpec() { BaseAddress = txt };
			}
		}
	}

}
