using ETL.DTO;
using ETL.Service.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ETL.Service.Implementations
{
    public class UI : IUI
    {
        ETLService _etlService;
        private CancellationTokenSource _cancelSource;
        StringBuilder _menu;
        System.Timers.Timer _timer;
        ETLSettings _etlSettings;
        FileSystemWatcher _watcher;
        public UI(IOptions<ETLSettings> etlSettings, ETLService etlService)
        {
            _etlSettings = etlSettings.Value;
            _timer = new System.Timers.Timer();
            _timer.Enabled = false;
            _timer.AutoReset = true;
            _timer.Interval = 5000;
            _timer.Elapsed += CheckMidnight;
            _menu = new StringBuilder("8.Start ETL service\n");
            _menu.Append("9.Stop\n");
            _menu.Append("0.Выход\n");
            _watcher = new FileSystemWatcher();
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Created += ProcessPaymentTransactions;
            _etlService = etlService;
            _watcher.Path = _etlService.GetPathFolderA();
        }

        private async void ProcessPaymentTransactions(object sender, FileSystemEventArgs e)
        {
            await _etlService.StartProcess(_cancelSource.Token);
        }

        private async void CheckMidnight(object? sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public async Task Menu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(_menu);
                Console.Write("Enter number: ");
                char key = Console.ReadKey().KeyChar;
                Console.WriteLine($"\nKEY: [{key}]\n");
                switch (key)
                {
                    case '8':
                        _cancelSource = new CancellationTokenSource();
                        _timer.Start();
                        await _etlService.StartProcess(_cancelSource.Token);
                        _watcher.EnableRaisingEvents = true;
                        break;
                    case '9':
                        _cancelSource?.Cancel();
                        _timer.Stop();
                        _watcher.EnableRaisingEvents = false;

                        break;
                    case '0':
                        return;
                    default:
                        Console.WriteLine("Not found!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        continue;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
