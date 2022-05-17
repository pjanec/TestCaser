using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace TestCaser
{
	#pragma warning disable CS0649

	/// <summary>
	/// Specifies the file path to be used by other parts
	/// </summary>
	public class FileSpec
	{
		public string Path;	// file path (null if not used)
		public string Preset; // preset id
		public NewestFile Newest; // folder + mask (like "C:\*.txt")
		public string Watcher; // file watcher instance name

		public class NewestFile
		{
			public string Folder; // folder + mask (like "C:\*.txt")
			public bool Recursive;
		}

		public string GetPath()
		{
			if( !string.IsNullOrEmpty(Path) )
			{
				return Path;
			}

			if( !string.IsNullOrEmpty(Preset) )
			{
				var fname = $"{Context.FileSpecsFolder}\\{Preset}.json";
				var jsonStr = File.ReadAllText( fname );
				var spec = JsonConvert.DeserializeObject<FileSpec>( jsonStr );
				return spec.GetPath();
			}

			if( Newest != null )
			{
				var files = FindMatchingFileInfos( Newest.Folder, Newest.Recursive );
				var newestFileName = GetNewest( files );
				return newestFileName;
			}

			if( !string.IsNullOrEmpty(Watcher) )
			{
				return FileWatcher.Load(Watcher).WatchedPath;
			}

			throw new Exception($"Invalid FileSpec");
		}


		public static FileSpec From( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return From( jtok.Value<string>() );
			}
			else
			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<FileSpec>();
			}
			throw new Exception($"Invalid file spec '{jtok}'");
		}

		public static FileSpec From( string txt )
		{
			if( string.IsNullOrEmpty(txt) ) throw new Exception("Empty file spec");

			if( Tools.IsJsonObj(txt) )
			{
				return JsonConvert.DeserializeObject<FileSpec>( txt );
			}
			else // consider a path
			{
				return new FileSpec() { Path=txt };
			}
			throw new Exception($"Invalid file spec '{txt}'");
		}

		static FileInfo[] FindMatchingFileInfos( string pathWithMask, bool recursive )
		{
			var dir = System.IO.Path.GetDirectoryName( pathWithMask );
			var mask = System.IO.Path.GetFileName( pathWithMask );
			if( string.IsNullOrEmpty(mask) ) mask = "*.*"; //throw new Exception($"No file mask given in '{pathWithMask}'");
			if( string.IsNullOrEmpty( dir ) ) dir = Directory.GetCurrentDirectory();
			var dirInfo = new DirectoryInfo(dir);
			var enumOpts = new EnumerationOptions()
			{
				MatchType = MatchType.Win32,
				RecurseSubdirectories = recursive,
				ReturnSpecialDirectories = false
			};
			FileInfo[] files = dirInfo.GetFiles( mask, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			return files;
		}

		static string GetNewest( FileInfo[] files )
		{
			if( files.Length == 0 ) return null;
			Array.Sort( files, (x, y) => x.LastWriteTimeUtc.CompareTo( y.LastWriteTimeUtc ) );
			return files[files.Length-1].FullName;
		}

	}
}
