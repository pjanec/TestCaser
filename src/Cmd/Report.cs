using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{
	/// <summary>
	/// Produces a HTML report of given type.
	/// "results" = test results
	/// </summary>
	public class Report : BaseCmd
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		[JsonIgnore]		
		public override string Code => "report";
		public string ReportType;

		public override void ParseCmd( string[] cmd )
		{
			ReportType = cmd[1];
		}

		public override ExitCode Execute()
		{
			if( ReportType == "results" )
			{
				try
				{
					var model = new Models.Results.ModelLoader().Load();
					new TemplateProcessor(model).ProcessFile(
						Context.TemplatesFolder+"\\Results.scriban",
						Context.ResultFolder + "\\Results.html"
					);
					return ExitCode.Success;
				}
				catch( Exception ex )
				{
					Console.WriteLine(ex.Message);
					return ExitCode.Failure;
				}
			}
			log.Error($"Unknown report type: {ReportType}");
			return ExitCode.Failure;
		}
	}
}
