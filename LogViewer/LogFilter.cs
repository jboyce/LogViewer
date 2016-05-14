using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;

namespace LogViewer
{
    public static class LogFilter
    {
        public static Func<dynamic, bool> CreateFilter(string searchString, bool searchStringIsLambda)
        {
            if (!searchStringIsLambda)
                return CreateSimpleFilter(searchString);

            string scriptText = searchString;

            Assembly microsoftCSharpAssembly = typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly;
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            ScriptOptions options = ScriptOptions.Default
                .WithReferences(microsoftCSharpAssembly, thisAssembly)
                .WithImports("System", "LogViewer");
            var script = CSharpScript.Create<bool>(scriptText, options, globalsType: typeof(ScriptGlobals));
            ScriptRunner<bool> runner = script.CreateDelegate();

            Func<dynamic, bool> filter = logEntry => runner(new ScriptGlobals { x = logEntry }).Result;
            return filter;
        }   
        
        private static Func<dynamic, bool> CreateSimpleFilter(string searchString)
        {
            return (dynamic x) => x.Message.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) > -1;
        }     
    }

    public class ScriptGlobals
    {
        public dynamic x;
    }
}
