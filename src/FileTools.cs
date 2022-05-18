using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TestCaser
{
	static class FileTools
	{
		public static T GetSpec<T>( string id, string folder )
		{
			var name = id.EndsWith(".json") ? id : id+".json";
			var path = $"{folder}\\{name}";
			var jsonStr = File.ReadAllText( path );
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>( jsonStr );
		}
		
	}
}
