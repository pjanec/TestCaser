using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace TestCaser
{
    /// <summary>
    /// reads include files as relative to the master file they are included from
    /// </summary>
    public class MyIncludeFromDisk : ITemplateLoader
    {
        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            if( Path.IsPathRooted( templateName ) )
                return templateName;

            var absolutePath = Path.Combine(
                Path.GetDirectoryName( context.CurrentSourceFile ),
                templateName
            );

            return absolutePath;
        }

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            // Template path was produced by the `GetPath` method above in case the Template has 
            // not been loaded yet
            return File.ReadAllText(templatePath);
        }

		public async ValueTask<string> LoadAsync( TemplateContext context, SourceSpan callerSpan, string templatePath )
        {
            return await Task.Run( () => Load( context, callerSpan, templatePath ) );
        }

    }
}
