using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace TestCaser
{
	public class Tools
	{
		public static void RecursiveDelete(DirectoryInfo baseDir, bool deleteJustContent=false)
		{
			if (!baseDir.Exists)
				return;

			foreach (var dir in baseDir.EnumerateDirectories())
			{
				RecursiveDelete(dir);
			}
			var files = baseDir.GetFiles();
			foreach (var file in files)
			{
				file.IsReadOnly = false;
				file.Delete();
			}
			if( !deleteJustContent )
			{
				baseDir.Delete();
			}
		}

		public static string GetExeDir()
		{
			var assemblyExe = Assembly.GetEntryAssembly().Location;
			if( assemblyExe.StartsWith( "file:///" ) ) assemblyExe = assemblyExe.Remove( 0, 8 );
			return System.IO.Path.GetDirectoryName( assemblyExe );
		}

	}
}
