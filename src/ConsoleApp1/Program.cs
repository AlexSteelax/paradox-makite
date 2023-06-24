using MakItE.Core.Helpers;
using MakItE.Core.Models.Common;
using MakItE.Core.Parser;
using MakItE.Core.Processors;
using MakItE.Core.Processors.Configurations;
using MakItE.Core.Serializer;
using MakItE.Core.Services;
using Superpower.Model;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApp1
{
    internal class Program
    {
        class ResourceData
        {
            public string? Code;
            public string? Path;
        }
        class ResultData
        {
            public IEnumerable<IObject>? Nodes;
            public string? Url;
        }

        static async IAsyncEnumerable<ExecuteResult<ResourceData>> ProduceData()
        {
            IResource resource = new Ck3Resources();

            var files = resource.GetFiles();

            foreach (var file in files)
            {
                //if (!file.EndsWith("fp2_achievements.txt")) continue;

                var path = Path.Combine(resource.Root, file);

                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using var reader = new StreamReader(stream, Encoding.UTF8, true);

                var code = await reader.ReadToEndAsync();

                yield return ExecuteResult<ResourceData>.Create(new() { Code = code, Path = file }, true);
            }
        }
        static ValueTask<ExecuteResult<ResultData>> ProcessHandler(ResourceData data)
        {
            var pstate = TokenListParser.TryParse(data.Code!, out IEnumerable<IObject>? result, out string? error, out Position errorPosition);

            if (pstate)
            {
                var o = new ResultData { Nodes = result, Url = data.Path!.UrlNormalize() };
                return ValueTask.FromResult(ExecuteResult<ResultData>.Create(o, Diagnostic.Create($"Thread: {Environment.CurrentManagedThreadId} File: {data.Path}")));
            }
                

            return ValueTask.FromResult(ExecuteResult<ResultData>.Create(Diagnostic.Create($"Thread: {Environment.CurrentManagedThreadId} File: {data.Path}\n{error}", Microsoft.CodeAnalysis.DiagnosticSeverity.Error)));
        }
        static ValueTask LogHandler(Diagnostic diagnostic)
        {
            if (diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Green;
            ///
            if (diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                Console.WriteLine(diagnostic.Description);
            ///
            return ValueTask.CompletedTask;
        }
        static async Task<ProcessResult<IEnumerable<ResultData>>> RunMakItE()
        {
            var result = await FileProcessor<ResourceData, ResultData>
                .Create()
                .ConfigureProducer(ProduceData)
                .ConfigureProcessor(ProcessHandler)
                .ConfigureLogger(LogHandler)
                .RunAsync();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Totals: {result.Totals}");
            Console.WriteLine($"Errors: {result.Errors}");
            Console.WriteLine($"Execution time: {result.Elapsed.TotalSeconds} sec");

            return result;
        }

        static async Task Main(string[] args)
        {
            /*
            var file = "D:\\Development\\test_mod\\ClassLibrary1\\ClassLibrary1\\Class1.cs";
            var result = EasyRoslyn
                .CreateCSharpBuilder(OutputKind.DynamicallyLinkedLibrary)
                .ConfigureReferences(s => s
                    .UseNetStandard()
                    .UseSystemRuntime()
                    .UseReference(typeof(Console))
                    .UseReference(typeof(Debugger)))
                .ConfigureCompilationOptions(s => s
                    .WithCurrentPlatform()
                    .WithOptimizationLevel(OptimizationLevel.Debug))
                .ConfigureEmitOptions(s => s
                    .WithDebugInformationFormat(DebugInformationFormat.PortablePdb))
                .ConfigureSources(s => s
                    .FromFile(file))
                .Build()
                .TryLoad(out Assembly assembly);

            dynamic instance = assembly.CreateInstance("ClassLibrary1.Class1");

            //while (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    Thread.Sleep(TimeSpan.FromSeconds(1)); //Or Task.Delay()
            //}

            Console.WriteLine("Debugger is attached!");
            
            instance.Print();
            */

            //for(var i = 0; i < 20; i++)
            //{
            //    Console.WriteLine($"Step {i}");
                await RunMakItE().ConfigureAwait(false);
            //}
            /*
            Console.WriteLine("Next 1");
            var result = await RunMakItE().ConfigureAwait(false);
            Console.WriteLine("Next 2");
            Console.WriteLine("Press any key...");
            _ = await RunMakItE().ConfigureAwait(false);
            Console.WriteLine("Next 3");
            Console.WriteLine("Press any key...");
            _ = await RunMakItE().ConfigureAwait(false);
            */
            /*
            var outDir = @"D:\test";

            foreach (var data in result.Result!)
            {
                var file = Path.Combine(outDir, data.Url!);
                var dir = Path.GetDirectoryName(file);


                Directory.CreateDirectory(dir!);

                using var stream = File.OpenWrite(file);

                await PdxSerializer.SerializeToAsync<CK3Serializer>(stream, data.Nodes!);

                await stream.FlushAsync();

                stream.Close();
            }
            */
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}