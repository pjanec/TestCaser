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
	public partial class FileWatcher
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

		string FileIdToFileName( string fileLocator )
		{
			var spec = FileSpec.FromId( fileLocator );
			return spec.GetFilePath();
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
