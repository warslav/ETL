using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Service.Implementations
{
    public class ETLService
    {
        FileService _fileService;

        public ETLService(FileService fileService)
        {
            _fileService = fileService;
        }

        public async Task StartProcess(CancellationToken cancelToken)
        {
            if (_fileService.GetFiles())
            {
                if (_fileService.ReadCSV())
                    Console.WriteLine("CSV files have been read");
                if (_fileService.ReadTXT())
                    Console.WriteLine("TXT files have been read");
            }
        }
        public string GetPathFolderA()
        {
            return _fileService.PathFolderA;
        }
    }
}
