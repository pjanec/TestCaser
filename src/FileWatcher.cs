using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

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
		string _id;	// watcher id
		string _watchedFileName;
		string _recFileName; // file where we save the file name and offset
		long _startOffset = 0;

		public string WatchedPath => _watchedFileName;

		public enum Mode
		{
			Load,
			Create,
			LoadOrCreate
		}

		public FileWatcher( string id, string path, Mode mode )
		{
			// if id null then we use the hash of the locator as the id
			if( string.IsNullOrEmpty(id) )
			{
				id = Tools.ComputeMd5Hash( path );
			}

			_id = id;

			_recFileName = GetWatchFileRecName(_id);

			if( mode == Mode.LoadOrCreate )
			{
				if( File.Exists( _recFileName ) )
					Load();
				else
					Create( path );
			}
			else
			if( mode == Mode.Create )
				Create( path );
			else
				Load();
		}

		static string GetWatchFileRecName( string fileId )
		{
			return $"{Context.WatchedFilesFolder}\\{Context.Instance.Case}-{fileId}.txt";
		}

		public void MoveToEnd()
		{
			using( var fs = File.Open( _watchedFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) )
			{
				fs.Seek(0, SeekOrigin.End);
				_startOffset = fs.Position;
			}
		}

		void Create( string path )
		{
			Directory.CreateDirectory( Context.WatchedFilesFolder );
			_watchedFileName = path;
			if( string.IsNullOrEmpty(_watchedFileName) ) throw new Exception($"No file'");

			//// remember current length so we do not start reading some 
			//using( var fs = File.Open( _watchedFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) )
			//{
			//	fs.Seek(0, SeekOrigin.End );
			//	_startOffset = fs.Position;
			//}
			//Save();

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
				_watchedFileName,
				_startOffset.ToString() // start offset
			};

			// create a watched file record
			File.WriteAllLines( _recFileName, lines );;
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
