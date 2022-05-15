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
		Args _args;

		public class Args
		{
			/// <summary>
			/// success if no line matching the regex
			/// </summary>
			public bool NotMatch;
		}

		public RegEx( string regexId, Args args )
		{
			_args = args;
			var fname = $"{Context.RegExFolder}\\{regexId}";
			var pattern = File.ReadAllText( fname );
			_re = new Regex( pattern );
		}

		public bool Search( List<string> lines )
		{
			// if at least one line needs to match, return false if one does not
			// if niether line should match, return false if one does
			bool retIfMatch = !_args.NotMatch; 

			// at least one line should match
			foreach( var line in lines )
			{
				var m = _re.Match( line );
				if( m.Success )
					return retIfMatch;
			}
			return !retIfMatch;
		}


	}
}
