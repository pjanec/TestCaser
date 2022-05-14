using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestCaser
{
	public class Watchf
	{
		Context _ctx = Context.Instance;
		string _fileId;
		string _watchedFileName;
		string _recFileName;
		long _startOffset = 0;

		public Watchf( string fileId, string fileLocator=null )
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
			// FIXME: some translation
			return fileLocator;
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
