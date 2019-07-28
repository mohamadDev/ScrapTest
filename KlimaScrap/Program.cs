using System;
using System.Diagnostics;
using System.Linq;

namespace KlimaScrap
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args == null)
            {
                LogSystem.Log($"null arg error {DateTime.Now} ");
                return 1;
            }

            if (args.Length < 1)
            {
                LogSystem.Log($"wrong arg error {DateTime.Now} ");
                return 2;
            }
            try
            {
                if (args.Length > 1)
                {
                    if (args[1] == "y")
                    {
                        new StatioManager().IngestStations();
                    }
                }
                var files = new ScrapInitializer(args[0]).Init();
                if (!files.Any())
                {
                    LogSystem.Log($"No new file {DateTime.Now} thread Slept");
                }
                Stopwatch sto= Stopwatch.StartNew();
                var flproc=new FileProcessorUnit(args[0],files);
                flproc.Process();
                sto.Stop();
                Console.WriteLine(sto.Elapsed.ToString("G"));
            }
            catch (Exception e)
            {
                LogSystem.Log($"An fatal Error happend {DateTime.Now}");
                LogSystem.Log(e.Message);
                return 3;
            }
            
Console.ReadKey();
            return 0;
        }
    }
}
