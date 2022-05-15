using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using NLog;
using System.Text.RegularExpressions;

namespace TestCaser
{
	static class Functions
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		static void print( string s )
		{
			log.Info( s );
		}

		static void log_error( string msg )
		{
			log.Error( msg );
		}

		static void log_warning( string msg )
		{
			log.Warn( msg );
		}

		static void log_info( string msg )
		{
			log.Info( msg );
		}

		static void log_debug( string msg )
		{
			log.Debug( msg );
		}

		static void os_mkdir( string path )
		{
			Directory.CreateDirectory( path );
		}

		static string os_getcwd()
		{
			return Directory.GetCurrentDirectory();
		}

		public static bool os_isdir(string fileName)
		{
			return Directory.Exists( fileName );
		}

		static bool os_isfile(string fileName)
		{
			return File.Exists( fileName );
		}

		static int os_execute( string command )
		{
			Process process = new Process();
			process.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec"); // "cmd.exe";
			process.StartInfo.Arguments = "/c"+command;

			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();

			try
			{
				process.Start();
			}
			catch (Exception ex)
			{
				throw new Exception( $"Failed to run '{process.StartInfo.FileName}' with args '{process.StartInfo.Arguments}'. {ex.Message}" );
			}

			process.WaitForExit();

			return process.ExitCode;
		}

		// returns array
		//   [0] = return code (int)
		//   [1] = stdout (string)
		//   [2] = stderr (string)
		static IEnumerable<object> os_outputof( string command )
		{
			Process process = new Process();
			process.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec"); // "cmd.exe";
			process.StartInfo.Arguments = "/c"+command;

			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;

			try
			{
				process.Start();
			}
			catch (Exception ex)
			{
				throw new Exception( $"Failed to run '{process.StartInfo.FileName}' with args '{process.StartInfo.Arguments}'. {ex.Message}" );
			}

			process.WaitForExit();

			var stdOutSB = new StringBuilder();
			while (!process.StandardOutput.EndOfStream)
			{
				string line = process.StandardOutput.ReadLine();
				stdOutSB.AppendLine( line );
			}

			var stdErrSB = new StringBuilder();
			while (!process.StandardError.EndOfStream)
			{
				string line = process.StandardError.ReadLine();
				stdErrSB.AppendLine( line );
			}

			return new List<object>() {
				process.ExitCode,
				stdOutSB.ToString(),
				stdErrSB.ToString()
			};
		}

		static string os_getenv( string varname )
		{
			return Environment.GetEnvironmentVariable( varname );
		}

		static void os_setenv( string varname, string value )
		{
			Environment.SetEnvironmentVariable( varname, value );
		}

		static void os_chdir( string path )
		{
			Directory.SetCurrentDirectory( path );
		}

		public static void os_remove( string path )
		{
			File.Delete( path );
		}

		public static void os_rmdir( string path )
		{
			Tools.RecursiveDelete( new DirectoryInfo( path ) );
		}


		static bool os_copyfile( string src, string dest )
		{
			try
			{
				File.Copy( src, dest, overwrite: true );
				return true;
			}
			catch
			{
				return false;
			}
		}


		// Returns the canonical absolute path of a filename.
		static string path_getabsolute( string path )
		{
			return Path.GetFullPath( path );
		}

		// Returns the base file portion of a path, with the directory and file extension removed.
		static string path_getbasename( string path )
		{
			return Path.GetFileNameWithoutExtension( path );
		}

		// Returns the file extension portion of a path.
		static string path_getextension( string path )
		{
			return Path.GetExtension( path );
		}

		// Returns the directory portion of a path, with any file name removed.
		static string path_getdirectory( string path )
		{
			return Path.GetDirectoryName( path );
		}

		// Returns the file name and extension, with any directory information removed.
		static string path_getname( string path )
		{
			return Path.GetFileName( path );
		}

		// computes a relative path from one directory to another.
		public static string path_getrelative( string filespec, string folder )
		{
			//https://stackoverflow.com/questions/703281/getting-path-relative-to-the-current-working-directory
			Uri pathUri = new Uri( Path.GetFullPath( filespec ));
			// Folders must end in a slash
			if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				folder += Path.DirectorySeparatorChar;
			}
			Uri folderUri = new Uri( Path.GetFullPath(folder) );
			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}

		// Determines if a given file system path is absolute.
		static bool path_isabsolute( string path )
		{
			return Path.IsPathRooted( path );
		}

		//static string path_normalize( string path )
		//{
		//	// TODO!
		//}

		static string io_readfile( string path )
		{
			return File.ReadAllText( path );
		}

		static void io_writefile( string path, string contents )
		{
			File.WriteAllText( path, contents );
		}

		// Returns the file extension portion of a path.
		static string path_replaceextension( string path, string extension )
		{
			return Path.ChangeExtension( path, extension );
		}

		static string path_join( string leading, string trailing )
		{
			return Path.Combine( leading, trailing );
		}

		//// Takes a path which is relative to one location and makes it relative to another location instead.
		////   c:\dir1\sub\file.txt
		////   c:\dir2\
		////
		////	 relativePath = ..\file.txt
		////   oldBase = c:\dir1\sub
		////   newBase = c:\dir2
		////   result = ..\dir1\sub\file.txt
		////  
		//static string path_rebase( string relativePath, string oldBase, string newBase )
		//{
		//	oldBase = Path.GetFullPath(oldBase);
		//	newBase = Path.GetFullPath(newBase);
		//}

		// converts all non-alphanumeric characters into undersores
		static string tools_underscorize( string str )
		{
			var sb = new StringBuilder();
			foreach( var c in str )
			{
				if( 
					(c >= 'a' && c <= 'z') ||
					(c >= 'A' && c <= 'Z') ||
					(c >= '0' && c <= '9')
				)
				{
					sb.Append( c );
				}
				else
				{
					sb.Append( '_' );
				}
			}
			return sb.ToString();
		}


		/// <summary>
		/// Creates a read-only property in given root script object,
		/// intended for importing of custom functions that should be accessible
		/// from the script via this 'namespace' property.
		/// </summary>
		public static ScriptObject CreateNamespace( ScriptObject parent, string name )
		{
			var so = new ScriptObject();
			parent[name] = so; 
			parent.SetReadOnly(name, true );
			return so;
		}
		
		public static void Register( ScriptObject parent )
		{
			parent.Import("print", new Action<string>( (string str) => print(str) ));

			{
				var so = CreateNamespace( parent, "log" );
				so.Import("error", new Action<string>( (string str) => log_error(str) ));
				so.Import("warning", new Action<string>( (string str) => log_warning(str) ));
				so.Import("info", new Action<string>( (string str) => log_info(str) ));
				so.Import("debug", new Action<string>( (string str) => log_debug(str) ));
			}

			{
				var so = CreateNamespace( parent, "os" );
				so.Import("mkdir", new Action<string>( (string str) => os_mkdir(str) ));
				so.Import("getcwd", new Func<string>(() => os_getcwd()));
				so.Import("isfile", new Func<string, bool>((string path) => os_isfile(path)));
				so.Import("isdir", new Func<string, bool>((string path) => os_isdir(path)));
				so.Import("execute", new Func<string, int>((string command) => os_execute(command)));
				so.Import("outputof", new Func<string, IEnumerable<object>>((string command) => os_outputof(command)));
				so.Import("chdir", new Action<string>( (string path) => os_chdir(path) ));
				so.Import("rmdir", new Action<string>( (string path) => os_rmdir(path) ));
				so.Import("remove", new Action<string>( (string path) => os_remove(path) ));
				so.Import("realpath", new Func<string, string>((string path) => path_getabsolute(path)));
				so.Import("getenv", new Func<string, string>((string varname) => os_getenv(varname)));
				so.Import("setenv", new Action<string, string>((string varname, string value) => os_setenv(varname, value)));
				so.Import("copyfile", new Func<string, string, bool>((string src, string dest) => os_copyfile(src, dest)));
			}

			{
				var so = CreateNamespace( parent, "path" );
				so.Import("getabsolute", new Func<string, string>((string path) => path_getabsolute(path)));
				so.Import("getbasename", new Func<string, string>((string path) => path_getbasename(path)));
				so.Import("getextension", new Func<string, string>((string path) => path_getextension(path)));
				so.Import("getdirectory", new Func<string, string>((string path) => path_getdirectory(path)));
				so.Import("getname", new Func<string, string>((string path) => path_getname(path)));
				so.Import("getrelative", new Func<string, string, string>((string file, string dir) => path_getrelative(file, dir)));
				so.Import("isabsolute", new Func<string, bool>((string path) => path_isabsolute(path)));
				so.Import("join", new Func<string, string, string>((string leading, string trailing) => path_join(leading, trailing)));
				so.Import("replaceextension", new Func<string, string, string>((string path, string ext) => path_replaceextension(path, ext)));
			}

			{
				var so = CreateNamespace( parent, "io" );
				so.Import("readfile", new Func<string, string>((string path) => io_readfile(path)));
				so.Import("writefile", new Action<string, string>((string path, string contents) => io_writefile(path, contents)));
			}

			{
				var so = CreateNamespace( parent, "tools" );
			}

		}
	}
}
