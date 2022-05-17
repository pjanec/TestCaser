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
	public class FileRegEx
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

		public FileRegEx( string regexId, Args args )
		{
			_args = args;
			var spec = RegexTools.GetSpec( regexId );
			_re = spec.Regex;
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
