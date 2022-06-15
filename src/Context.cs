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

		public static string ResultFolder = "Results";
		public static string CurrentStatusFolder = "Status";
		public static string CaseFileName = CurrentStatusFolder+"\\case.txt";
		public static string PhaseFileName = CurrentStatusFolder+"\\phase.txt";
		public static string WatchedFilesFolder = CurrentStatusFolder+"\\WatchedFiles";
		public static string AreaSpecsFolder = "Specs\\Area";
		public static string RegExSpecsFolder = "Specs\\Regex";
		public static string WebreqSpecsFolder = "Specs\\Webreq";
		public static string AddrSpecsFolder = "Specs\\Addr";
		public static string ScriptexSpecsFolder = "Specs\\Scriptex";
		public static string WindowSpecsFolder = "Specs\\Window";
		public static string FileSpecsFolder = "Specs\\File";
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
				System.IO.Directory.CreateDirectory( CurrentStatusFolder );

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
