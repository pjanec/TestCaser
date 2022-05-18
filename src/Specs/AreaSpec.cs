using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WindowsDisplayAPI;
using WindowsDisplayAPI.DisplayConfig;


namespace TestCaser
{
#pragma warning disable CS0649

	/// <summary>
	/// Specifies the coordinates of the screen area to be used by other parts
	/// </summary>
	public class AreaSpec
	{
		public string Preset;
		public JToken Rect;  // rectangle on the screen or window or desktop
		public ScreenArea Screen; // rectangle on given screen
		public JToken Window; // windowSpec

		public class MyRectangle
		{
			public JToken X;  // 12, "50%"
			public JToken Y;
			public JToken W;
			public JToken H;  // 640
		}

		public class ScreenArea
		{
			public int Id; // 0=whole desktop
		}


		public static Rectangle ResolveAbsRel( JToken any, Rectangle reference )
		{
			if( any.Type == JTokenType.Array )
			{
				var jarr = any as JArray;
				if( jarr.Count != 4 ) throw new Exception($"rectangle needs 4 coordinates: {any}");
				return new Rectangle()
				{
					X = AbsRel( jarr[0], reference.Width ),
					Y = AbsRel( jarr[1], reference.Height ),
					Width = AbsRel( jarr[2], reference.Width ),
					Height = AbsRel( jarr[3], reference.Height ),
				};
			}
			
			if( any.Type == JTokenType.Object )
			{
				var jobj = any as JObject;
				var myRect = jobj.ToObject<MyRectangle>();

				return new Rectangle()
				{
					X = AbsRel( myRect.X, reference.Width ),
					Y = AbsRel( myRect.Y, reference.Height ),
					Width = AbsRel( myRect.W, reference.Width ),
					Height = AbsRel( myRect.H, reference.Height ),
				};
			}

			throw new Exception($"Invalid rectangle: {any}");
		}

		static int AbsRel( JToken x, int size )
		{
			if( x.Type == JTokenType.Integer ) return x.ToObject<int>();
			if( x.Type == JTokenType.Float ) return x.ToObject<int>();
			if( x.Type == JTokenType.String )
			{
				var str = x.ToObject<string>().Trim();
				if( str.EndsWith('%') )
				{
					if( float.TryParse(str[0..^1], out var val))
					{
						return (int)(val/100f * size);
					}
					throw new Exception($"Invalid percentage value: {x}");

				}
				else
				{
					if( float.TryParse(str, out var val))
					{
						return (int)val;
					}
					throw new Exception($"Invalid numeric value: {x}");
				}
			}
			throw new Exception($"Invalid value: {x}");
		}

		// returns physical coordinates
		public Rectangle GetRect()
		{
			if( !string.IsNullOrEmpty(Preset) )
			{
				var spec = FileTools.GetSpec<AreaSpec>( Preset, Context.AreaSpecsFolder );
				return spec.GetRect();
			}

			if( Window != null )
			{
				var winSpec = WindowSpec.From( Window );
				var hWnd = winSpec.GetWindow();
				var refRect = GetAreaRectByHwnd( hWnd );
	
				if( Rect == null )
					return refRect;
				else
					return ResolveAbsRel( Rect, refRect );
			}

			if( Screen != null )
			{
				var refRect = GetScreenRect( Screen.Id );
				if( refRect.Size.Width == 0 || refRect.Size.Width == 0 )
					throw new Exception("Invalid screen number");

				if( Rect == null )
					return refRect;
				else
					return ResolveAbsRel( Rect, refRect );
			}

			// rect on the whole desktop
			if( Rect != null )
			{
				var refRect = GetAllScreensRect();
				return ResolveAbsRel( Rect, refRect );
			}

			throw new Exception("Invalid area spec");
		}

		public static Rectangle GetAreaRectByHwnd( IntPtr hWnd )
		{
			if( hWnd == IntPtr.Zero )
				throw new Exception($"No window found.");
			if( !WinTools.GetWindowClientRectPhysical( hWnd, out var physicalRect ) ) 
				throw new Exception($"Failed to get client area coordinates.");
			if( physicalRect.Width == 0 || physicalRect.Height == 0 )
				throw new Exception($"Window client area not visible.");

			return physicalRect;
		}

		public static AreaSpec From( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return new AreaSpec() { Preset = jtok.Value<string>() };
			}
			else
			if (jtok.Type == JTokenType.Object)
			{
				return (jtok as JObject).ToObject<AreaSpec>();
			}
			else
			if (jtok.Type == JTokenType.Array)
			{
				return new AreaSpec() { Rect = jtok };
			}
			throw new Exception("Invalid file spec");
		}

		public static AreaSpec From( string txt )
		{
			if (string.IsNullOrEmpty( txt )) throw new Exception( "Empty area spec" );

			if (Tools.IsJsonArr( txt ))
			{
				var jtok = JToken.Parse(txt);
				return From( jtok );
			}
			if (Tools.IsJsonObj( txt ))
			{
				return JsonConvert.DeserializeObject<AreaSpec>( txt );
			}
			else
			{
				return new AreaSpec() { Preset = txt };
			}
		}

		/// <summary>
		/// Returns physical coordinates (usable for taking screenshot) of the bounding rectangle
		/// of all available screens
		/// </summary>
		/// <returns></returns>
		public static Rectangle GetAllScreensRect()
		{
			return PathInfo.GetActivePaths()
				.Select( pi => new Rectangle( pi.Position, pi.Resolution ) )
				.Aggregate(Rectangle.Union);
		}

		public static Rectangle GetScreenRect( int screenId )
		{
			var pi =  (from x in PathInfo.GetActivePaths() where x.DisplaySource.SourceId == screenId select x).FirstOrDefault();
			if( pi == null ) return new Rectangle();
			return new Rectangle( pi.Position, pi.Resolution );
		}

		public static void Test()
		{
			 //Debug.Assert( FromId("[1,2,3,4]").GetAreaPhysicalRect() == new Rectangle(1,2,3,4) );
			 {
				 var spec = From("[1,2,3,4]");
				 var rect = spec.GetRect();
			 }

			 {
				var spec = From("['50%','2','90%','4']");
				var rect = spec.GetRect();
			}

			 {
				 var spec = From("{Rect:[1,2,3,4]}");
				 var rect = spec.GetRect();
			 }

			 {
				 var spec = From("{Screen:{Id:1}}");
				 var rect = spec.GetRect();
			 }

			 {
				 var spec = From("{Screen:{Id:1,Rect:[1,2,3,4]}}");
				 var rect = spec.GetRect();
			 }

			 {
				 var spec = From("{Window:{Id:{ByTitle:{Regex:{Pattern:'aaa'}}}}}");
				 var rect = spec.GetRect();
			 }

		}

	}
}
