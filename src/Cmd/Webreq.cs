using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestCaser.Cmd
{

	/// <summary>
	/// Makes a screenshot of given area and saves it as an image file
	/// </summary>
	public class Webreq : BaseCmd
	{
		public JToken Addr;
		public JToken Request;
		public JToken Expr; // expression to evaluate the status (applied only if regex returns any match)

		public override string Brief => Addr+"; "+Request;

		public override void ParseCmd( string[] cmd )
		{
			Addr = Tools.ToJToken( cmd[1] );	// http://server:port
			Request = Tools.ToJToken( cmd[2] );		// api/blabla?x=3
			if( cmd.Length > 3 && Tools.IsJsonObj( cmd[3] ) ) JsonConvert.PopulateObject( cmd[3], this );
		}

		public class Result : BaseResult
		{
			public string Expr;	// the expression evaluated
			public ScriptexDataModel Response;
		}

		public class ScriptexDataModel
		{
			public int StatusCode;
			public string ReasonPhrase;
			public string Body;
			public object Json;

			public ScriptexDataModel( HttpResponseMessage response )
			{
				StatusCode = (int) response.StatusCode;
				ReasonPhrase = response.ReasonPhrase;
				var task = Task.Run( async () => { return await response.Content.ReadAsStringAsync(); } );
				task.Wait();
				Body = task.Result;
				try
				{
					var jtoken = JToken.Parse( Body );
					if( jtoken.Type == JTokenType.Array )
					{
						Json = new object[] { (jtoken as JArray) };
					}
					if( jtoken.Type == JTokenType.Object )
					{
						Json = (jtoken as JObject).ToObject<Dictionary<string, object>>();
					}
				}
				catch
				{
					Json = null;
				}
			}
		}

		public override ExitCode Execute()
		{
			try
			{
				var req = WebreqSpec.From( Request );

				var client = new HttpClient();
				client.BaseAddress = new Uri( AddrSpec.From(Addr).GetBaseAddress() );
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage response = null;
				var method = req.GetMethod().ToLower();
				var url = req.GetUrl();
				var body = req.GetBody();
				if( string.IsNullOrEmpty(method) || method == "get" )
				{
					Task.Run(async () => { response = await client.GetAsync( url ); }).Wait();
				}
				else
				if( method == "put" )
				{
					var content = new StringContent( req.GetBody(), Encoding.UTF8 );
					Task.Run(async () => { response = await client.PutAsync( url, content ); }).Wait();
				}
				else
				if( method == "post" )
				{
					var content = new StringContent( body, Encoding.UTF8 );
					Task.Run(async () => { response = await client.PostAsync( url, content ); }).Wait();
				}
				else
				{
					throw new Exception($"invalid method '{method}' in web request");
				}

				// apply expression on the match
				if( Expr != null )
				{
					var exprSpec = ScriptexSpec.From( Expr );
					var exprStr = exprSpec.GetExpr();
					var model = new ScriptexDataModel( response );
					var evaluator = new ScriptexEvaluator();
					object result;
					try
					{
						result = evaluator.Evaluate( exprStr, null, model );
					}
					catch( Exception ex )
					{
						Results.Add( this, new Result()
						{
							Status=EStatus.ERROR,
							Error = ex.Message,
							Expr = exprStr,
							Response = model
						});
						return ExitCode.Error;
					}

					if( result is bool )
					{
						if( !((bool)result) ) // not passed
						{
							Results.Add( this, new Result()
							{
								Status=EStatus.FAIL, Error="Expression returned false",
								Expr = exprStr,
								Response = model
							});
							return ExitCode.Failure;
						}
						else
						{
							Results.Add( this, new Result()
							{
								Status=EStatus.OK,
								Expr = exprStr,
								Response = model
							});
							return ExitCode.Success;
						}
					}
					else
					{
						Results.Add( this, new Result()
						{
							Status=EStatus.ERROR, Error="Expression did not return a bool value",
							Expr = exprStr,
							Response = model
						});
						return ExitCode.Error;
					}
				}
				else
				{
					Results.Add( this, new Result()
					{
						Status=EStatus.OK,
						Response = new ScriptexDataModel( response )
					});
					return ExitCode.Success;
				}
			}
			catch(Exception ex)
			{
				Results.Add( this, new Result() { Status = EStatus.ERROR, Error = ex.Message } );

				return ExitCode.Failure;
			}
		}
	}
}
