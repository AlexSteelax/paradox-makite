using System.Text;
using System.Linq;
using System.IO;
using static System.Net.WebRequestMethods;
using Superpower.Model;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using Superpower.Parsers;
using MakItE.Core;
using MakItE.Core.Tokenizer;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            var list = new List<IX>();

            list.Add(new X());
            list.Add(new Xx<int> { Value = 5 });

            var x1 = list[0] as X;
            var x2 = list[1] as Xx<int>;
            var x3 = 0;
            */
            //JsonParser.Program.Main2();
            /*
            var text = @"
# Events for handling the Feast activity
@xz_12_x = ""123""
@y = 123
@z = -12.5
namespace = ""xxx""
test = 5
test_my = {
    hidden = yes
    scope = combat_side
    any_in_list = {
		list = prisoners_of_war
		OR = {
			this = root.enemy_side.side_primary_participant
			is_heir_of = root.enemy_side.side_primary_participant
		}
		troops_ratio <= 0.5
		num_enemies_killed >= 10000
		AND = {
			percent_enemies_killed >= 75
			combat = {
				num_total_troops >= 20000
			}
		}
        10 = {}
	}
}
";
            */
            //var text = "@[cultural_maa_extra_ai_score + 20]";
            var root = @"D:\SteamLibrary\steamapps\common\Crusader Kings III\game";
            //var root = @"D:\SteamLibrary\steamapps\workshop\content\1158310\2887120253";
            //var root = @"D:\SteamLibrary\steamapps\workshop\content\1158310\2920116721";
            
            var folders = new (string Dir, string Mask)[]
            {
                (@"common", "*.txt"),
                (@"events", "*.txt"),
                (@"gui", "*.gui")
            };
            
            /*
            var root = @"D:\\SteamLibrary\\steamapps\\common\\Stellaris";

            var folders = new (string Dir, string Mask)[]
            {
                (@"common", "*.txt"),
                (@"events", "*.txt"),
                (@"interface", "*.gui")
            };
            */
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var result = new ConcurrentBag<(string Path, bool HasValue, string ErrorPosition)>();
            
            var files = folders.SelectMany(s => Directory.EnumerateFiles(Path.Combine(root, s.Dir), s.Mask, SearchOption.AllDirectories));
            //files = new string[] { files.First() };

            Parallel.ForEach(files, (filePath, state) =>
            {
                var text = ReadFile(filePath);
                var relPath = Path.GetRelativePath(root, filePath);
                /*
                text = @"
scope:x_y.10
";
         */       
                var tokens = TokenParser.Instance.TryTokenize(text);

                if (tokens.HasValue)
                {
                    result.Add((relPath, true, string.Empty));
                }
                else
                {
                    result.Add((relPath, false, tokens.ErrorPosition.ToString()));
                }

                //ParadoxParser2.Parse(tokens);
                //var p = ParadoxParser2.TryParse(text, out object? value, out string error, out Position errorPosition);

                //state.Break();
            });

            foreach(var item in result)
            {
                if (item.HasValue)
                {
                    //nothing
                    //Console.WriteLine($"+ {item.Path}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"- {item.Path}");
                    Console.WriteLine(item.ErrorPosition);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            watch.Stop();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Execution time: {watch.ElapsedMilliseconds / 1000} sec");
            Console.WriteLine($"Total files: {result.Count}");
            Console.WriteLine("Press any key");
            Console.ReadKey();
            
        }

		static string ReadFile(string filePath)
		{
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream, Encoding.UTF8, true);

            return reader.ReadToEnd();
        }
    }
}