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
		public JToken Rect;  // rectangle on the whole desktop (physical coords)
		public ScreenArea Screen; // rectangle on given screen
		public WindowArea Window; // rectangle in given window

		public class MyRectangle
		{
			public JToken X;  // 12, "50%"
			public JToken Y;
			public JToken Width;
			public JToken Height;  // 640
		}

		public class ScreenArea
		{
			public int Id; // 0=whole desktop
			public JToken Rect;
		}

		public class WindowArea
		{
			public JToken Id; // "windowLocatorName", {json object}
			public JToken Rect;
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
					Width = AbsRel( myRect.Width, reference.Width ),
					Height = AbsRel( myRect.Height, reference.Height ),
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
		public Rectangle GetAreaPhysicalRect()
		{
			if( Rect != null )
			{
				var refRect = GetAllScreensRect();
				return ResolveAbsRel( Rect, refRect );
			}

			if( Window != null )
			{
				var winSpec = WindowSpec.FromId( Window.Id );
				var hWnd = winSpec.GetWindow();
				var refRect = GetAreaRectByHwnd( hWnd );
	
				if( Window.Rect == null )
					return refRect;
				else
					return ResolveAbsRel( Window.Rect, refRect );
			}

			if( Screen != null )
			{
				var refRect = GetScreenRect( Screen.Id );
				if( refRect.Size.Width == 0 || refRect.Size.Width == 0 )
					throw new Exception("Invalid screen number");

				if( Screen.Rect == null )
					return refRect;
				else
					return ResolveAbsRel( Screen.Rect, refRect );
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

		public static AreaSpec FromId( JToken jtok )
		{
			if (jtok.Type == JTokenType.String)
			{
				return FromId( jtok.Value<string>() );
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

		public static AreaSpec FromId( string id )
		{
			if (string.IsNullOrEmpty( id )) throw new Exception( "Empty area locator" );

			if (Tools.IsJsonArr( id ))
			{
				var jtok = JToken.Parse(id);
				return FromId( jtok );
			}
			if (Tools.IsJsonObj( id ))
			{
				return JsonConvert.DeserializeObject<AreaSpec>( id );
			}
			else
			{
				var fname = $"{Context.AreaSpecsFolder}\\{id}.json";
				var jsonStr = File.ReadAllText( fname );
				return JsonConvert.DeserializeObject<AreaSpec>( jsonStr );
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
				 var spec = FromId("[1,2,3,4]");
				 var rect = spec.GetAreaPhysicalRect();
			 }

			 {
				var spec = FromId("['50%','2','90%','4']");
				var rect = spec.GetAreaPhysicalRect();
			}

			 {
				 var spec = FromId("{Rect:[1,2,3,4]}");
				 var rect = spec.GetAreaPhysicalRect();
			 }

			 {
				 var spec = FromId("{Screen:{Id:1}}");
				 var rect = spec.GetAreaPhysicalRect();
			 }

			 {
				 var spec = FromId("{Screen:{Id:1,Rect:[1,2,3,4]}}");
				 var rect = spec.GetAreaPhysicalRect();
			 }

			 {
				 var spec = FromId("{Window:{Id:{ByTitle:{Regex:{Pattern:'aaa'}}}}}");
				 var rect = spec.GetAreaPhysicalRect();
			 }

		}

	}
}
