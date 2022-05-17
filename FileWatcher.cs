using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestCaser
{
	/// <summary>
	/// Gets list of lines appended from last query.
	/// File is identified using a symbolic Id. The actual file path
	/// is resolved using a 'locator' which cound be either the file path
	/// or a prescription of al algorithm how to find the path (for example newest file in some folder)
	/// </summary>
	public class FileWatcher
	{
		Context _ctx = Context.Instance;
		string _fileId;
		string _watchedFileName;
		string _recFileName;
		long _startOffset = 0;

		public FileWatcher( string fileId, string fileLocator=null )
		{
			_fileId = fileId;
			_recFileName = GetWatchFileRecName(_fileId);

			if( fileLocator!=null )
				Create(fileLocator);
			else
				Load();
		}

		string GetWatchFileRecName( string fileId )
		{
			return $"{Context.WatchedFilesFolder}\\{_ctx.Case}-{fileId}.txt";
		}

		void Create(string fileLocator )
		{
			Directory.CreateDirectory( Context.WatchedFilesFolder );
			_watchedFileName = FileIdToFileName( fileLocator );
			if( string.IsNullOrEmpty(_watchedFileName) ) throw new Exception($"No file matching the locator '{fileLocator}'");
		}

		void Load()
		{
			// create a watched file record
			var lines = File.ReadAllLines( _recFileName ).ToList();
			_watchedFileName = lines[0];
			_startOffset = 0;
			if( lines.Count > 1 )
			{
				long.TryParse( lines[1], out _startOffset );
			}
		}

		public void Save()
		{
			var lines = new List<string>()
			{
				FileIdToFileName( _watchedFileName ),
				_startOffset.ToString() // start offset
			};

			// create a watched file record
			File.WriteAllLines( _recFileName, lines );;
		}

		class JsonLocator
		{
			public string Newest; // folder + mask (like "C:\*.txt")
			public bool Recursive;
		}
		
		string FileIdToFileName( string fileLocator )
		{
			if( string.IsNullOrEmpty(fileLocator) ) throw new Exception("File locator is empty");
			fileLocator = fileLocator.Trim();

			// json?
			if( fileLocator.StartsWith('{') && fileLocator.EndsWith('}') )
			{
				return EvalJsonLocator( fileLocator );
			}
			else // assume fileLocator = file name
			{
				return fileLocator;
			}
		}

		string EvalJsonLocator( string jsonLoc )
		{
			var args = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonLocator>( jsonLoc );

			if( !string.IsNullOrEmpty(args.Newest) )
			{
				var files = FindMatchingFileInfos( args.Newest, args.Recursive );
				var newestFileName = GetNewest( files );
				return newestFileName;
			}

			throw new Exception($"Unrecognized file locator '{jsonLoc}'");
		}

		FileInfo[] FindMatchingFileInfos( string pathWithMask, bool recursive )
		{
			var dir = Path.GetDirectoryName( pathWithMask );
			var mask = Path.GetFileName( pathWithMask );
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

		string GetNewest( FileInfo[] files )
		{
			if( files.Length == 0 ) return null;
			Array.Sort( files, (x, y) => x.LastWriteTimeUtc.CompareTo( y.LastWriteTimeUtc ) );
			return files[files.Length-1].FullName;
		}

		// return the lines since last query
		public List<string> GetLines( bool fromBeginning=false )
		{
			using( var fs = File.Open( _watchedFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) )
			{
				if( !fromBeginning )
				{
					fs.Seek( _startOffset, SeekOrigin.Begin);
				}

				using( var sr = new StreamReader(fs) )
				{
					var lines = new List<string>();
					while (!sr.EndOfStream) 
					{
						lines.Add( sr.ReadLine() );
					}

					_startOffset = sr.GetActualPosition();

					return lines;
				}			
			}
		}

	}
}
