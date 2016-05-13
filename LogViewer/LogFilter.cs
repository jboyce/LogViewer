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
            string scriptText = searchStringIsLambda 
                ? searchString
                : string.Format("x.Message.Contains(\"{0}\")", searchString);
            
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
    }

    public class ScriptGlobals
    {
        public dynamic x;
    }
}
