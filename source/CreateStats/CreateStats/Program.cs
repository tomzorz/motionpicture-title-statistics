using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateStats
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string dataFileName = "titles.basics.csv";

            // grab file
            var cd = Directory.GetCurrentDirectory();
            while (Directory.GetFiles(cd,dataFileName).Count() != 1)
            {
                cd = Directory.GetParent(cd)?.FullName;
                if(cd == null) throw new Exception("Couldn't find source file!");
            }
            var filePath = Path.Combine(cd, dataFileName);

            Console.WriteLine("Found source file!");

            // title cleanup tools
            var trimBeginning = new[] {"A ", "AN ", "THE ", "EL ", "EIN ", "EINE ", "EN "};
            string CleanTitle(string original, IEnumerable<string> removedBeginnings)
            {
                var uc = original.Trim().ToUpperInvariant();
                foreach (var removedBeginning in removedBeginnings)
                {
                    if (!uc.StartsWith(removedBeginning)) continue;
                    return uc.Replace(removedBeginning, "");
                }
                return uc;
            }

            // our letters (27th is not A-Z)
            var letterCounts = new int[27];

            var lastPerc = 0.0;

            // data
            using (var fs = File.OpenRead(filePath))
            {
                using (var sr = new StreamReader(fs))
                {
                    // skip header line
                    await sr.ReadLineAsync();

                    // process
                    Console.WriteLine("Parsing data progress: ");
                    while (!sr.EndOfStream)
                    {
                        var line = await sr.ReadLineAsync();
                        var explo = line.Split('\t');

                        /*
                            0 tconst
                            1 titletype
                            2 primarytitle
                            3 originaltitle
                            4 isadult
                            5 startyear
                            6 endyear
                            7 runtimeminutes
                            8 genre
                         */

                        // skip adult content
                        if (explo[4] != "0") continue;

                        // gather data
                        var cleanTitle = CleanTitle(explo[2], trimBeginning);

                        // skip "EPISODE" if starts with
                        if(explo[1] == "tvEpisode" && cleanTitle.StartsWith("EPISODE")) continue;

                        var fchar = cleanTitle[0] - 'A';
                        if (fchar < 0 || fchar > 25) fchar = 26;
                        letterCounts[fchar] += 1;

                        var perc = fs.Position / (double) fs.Length * 100;
                        if (!(perc > lastPerc + 1.0)) continue;
                        lastPerc = perc;
                        Console.Write($"{perc:F0}% ... ");
                    }
                    Console.WriteLine();
                }
            }

            // output data
            Console.WriteLine("Outputting stats.md...");
            var statFile = Path.Combine(cd, "stats.md");
            if (File.Exists(statFile)) File.Delete(statFile);
            using (var fs = File.OpenWrite(statFile))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var total = (double)letterCounts.Sum();
                    await sw.WriteLineAsync("# IMDB Motion Picture Title Statistics");
                    await sw.WriteLineAsync();
                    await sw.WriteLineAsync($"_Generated at: {DateTime.Now:F}_");
                    await sw.WriteLineAsync();
                    await sw.WriteLineAsync($"Total motion picture title count: {total}");
                    await sw.WriteLineAsync();
                    await sw.WriteLineAsync("Letter | Count | Percentage");
                    await sw.WriteLineAsync("--- | ---:| ---:");
                    for (var index = 0; index < letterCounts.Length - 1; index++)
                    {
                        var letterCount = letterCounts[index];
                        await sw.WriteLineAsync($"{(char) (index + 'A')} | {letterCount} | {letterCount/total*100:F2}%");
                    }
                    await sw.WriteLineAsync($"not A-Z | {letterCounts[26]} | {letterCounts[26]/total*100:F2}%");
                }
            }
            Console.WriteLine("Done!");
        }
    }
}
