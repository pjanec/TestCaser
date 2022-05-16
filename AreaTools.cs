using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TestCaser
{
	static class AreaTools
	{
		public class Spec
		{
		}

		public static Spec GetSpec( string areaId )
		{
			var fname = $"{Context.AreaFolder}\\{areaId}.json";
			var jsonStr = File.ReadAllText( fname );
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Spec>( jsonStr );
		}
		
	}
}
