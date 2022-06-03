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
		public static string AreaSpecsFolder = "AreaSpecs";
		public static string RegExSpecsFolder = "RegexSpecs";
		public static string WebreqSpecsFolder = "WebreqSpecs";
		public static string AddrSpecsFolder = "AddrSpecs";
		public static string ScriptexSpecsFolder = "ScriptexSpecs";
		public static string WindowSpecsFolder = "WindowSpecs";
		public static string FileSpecsFolder = "FileSpecs";
		public static string PatternImgFolder = "PatternImages";
		public static string OutputImgFolder = ResultFolder+"\\Images";
		public static string TemplatesFolder = "Templates";

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

	}
}
