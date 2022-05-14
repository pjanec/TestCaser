using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace TestCaser
{
	public class RegEx
	{
		Regex _re;

		public RegEx( string regexId )
		{
			var fname = $"{Context.RegExFolder}\\{regexId}";
			var pattern = File.ReadAllText( fname );
			_re = new Regex( pattern );
		}

		public bool Search( List<string> lines )
		{
			foreach( var line in lines )
			{
				var m = _re.Match( line );
				if( m.Success )
					return true;
			}
			return false;
		}


	}
}
