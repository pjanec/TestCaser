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
		public class Spec
		{
		}

		public static Spec GetSpec( string fileId )
		{
			var fname = $"{Context.AreaFolder}\\{fileId}.json";
			var jsonStr = File.ReadAllText( fname );
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Spec>( jsonStr );
		}
		
	}
}
