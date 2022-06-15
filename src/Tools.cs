using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

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

		public static bool IsJsonObj( string str )
		{
			if( string.IsNullOrEmpty( str ) ) return false;
			return str.StartsWith('{') && str.EndsWith('}');
		}

		public static bool IsJsonArr( string str )
		{
			if( string.IsNullOrEmpty( str ) ) return false;
			return str.StartsWith('[') && str.EndsWith(']');
		}

		public static bool IsJson( string str )
		{
			return IsJsonObj(str) || IsJsonArr(str);
		}

		public static JToken ToJToken( string str )
		{
			if (IsJson( str )) // parse json
				return JToken.Parse( str );
			else // keep as string
				return new JValue( str );
		}

		public static string ComputeMd5Hash(string message)
		{
			using (var md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] input = Encoding.ASCII.GetBytes(message);
				byte[] hash = md5.ComputeHash(input);
	
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hash.Length; i++)
				{
					sb.Append(hash[i].ToString("X2"));
				}
				return sb.ToString();
			}
		}

		public static JToken RemoveEmptyChildren(JToken token)
		{
			if (token.Type == JTokenType.Object)
			{
				JObject copy = new JObject();
				foreach (JProperty prop in token.Children<JProperty>())
				{
					JToken child = prop.Value;
					if (child.HasValues)
					{
						child = RemoveEmptyChildren(child);
					}
					if (!IsEmpty(child))
					{
						copy.Add(prop.Name, child);
					}
				}
				return copy;
			}
			else if (token.Type == JTokenType.Array)
			{
				JArray copy = new JArray();
				foreach (JToken item in token.Children())
				{
					JToken child = item;
					if (child.HasValues)
					{
						child = RemoveEmptyChildren(child);
					}
					if (!IsEmpty(child))
					{
						copy.Add(child);
					}
				}
				return copy;
			}
			return token;
		}

		public static bool IsEmpty(JToken token)
		{
			return (token.Type == JTokenType.Null);
		}

		/// <summary>
		/// Replaces %ENVVAR% in a string with actual value of evn vars; undefined ones will be replaced with empty string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ExpandEnvVars( String str, bool leaveUnknown = false )
		{

			System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match( str, @"(%\w+%)" );

			while( match.Success )
			{
				string varName = match.Value.Replace( "%", "" ).Trim();
				string varValue = Environment.GetEnvironmentVariable( varName );

				bool replace = true;

				if( varValue == null )
				{
					if( leaveUnknown )	// do not replace, leave as is
					{
						replace = false;
					}
					else // replace the unknown var with empty string
					{
						varValue = String.Empty;
					}
				}

				if( replace )
				{
					str = str.Replace( match.Value, varValue );
				}
				match = match.NextMatch();
			}
			return str;
		}


	}
}
