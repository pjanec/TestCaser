using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestCaser
{
	public class Context
	{
		public static Context Instance;

		public static string CaseFileName = "case.txt";
		public static string PhaseFileName = "phase.txt";
		public static string ResultFolder = "Results";
		public static string WatchedFilesFolder = "WatchedFiles";
		public static string AreaFolder = "Areas";
		public static string RegExFolder = "RegEx";
		public static string PatternImgFolder = "PatternImages";
		public static string OutputImgFolder = ResultFolder+"\\Images";
		public static string TemplatesFolder = Tools.GetExeDir()+"\\Templates";

		public string Case
		{
			get
			{
				try { return File.ReadAllText(CaseFileName); }
				catch { return string.Empty; }
			}
			set
			{
				// remember case
				File.WriteAllText( CaseFileName, value );
			
				// clear phase
				File.WriteAllText( PhaseFileName, "");
			}
		}

		public string Phase
		{
			get
			{
				try { return File.ReadAllText(PhaseFileName); }
				catch { return string.Empty; }
			}
			set
			{
				// remember phase
				File.WriteAllText( PhaseFileName, value );
			}
		}

		public void AddResult( string statusCode, string cmdCode, params string[] args )
		{
			Directory.CreateDirectory( ResultFolder );
			var fname = $"{ResultFolder}\\{Case}.txt";
			var line = $"{statusCode}:{Phase}:{cmdCode}:{String.Join(':', args)}\n";
			File.AppendAllText( fname, line );
		}

	}
}
