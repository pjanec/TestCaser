using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace TestCaser
{
	/// <summary>
	/// Searches for regex in a list of lines.
	/// </summary>
	public class FileRegexSearcher
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

		public FileRegexSearcher( Regex re, Args args )
		{
			_args = args;
			_re = re;
		}

		public bool Search( List<string> lines, out Match match )
		{
			// if at least one line needs to match, return false if one does not
			// if niether line should match, return false if one does
			bool retIfMatch = !_args.NotMatch; 

			// at least one line should match
			foreach( var line in lines )
			{
				var m = _re.Match( line );
				if( m.Success )
				{
					match = m;
					return retIfMatch;
				}
			}
			match = null;
			return !retIfMatch;
		}


	}
}
