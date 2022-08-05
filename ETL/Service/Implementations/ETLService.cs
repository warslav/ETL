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

        public async Task StartProcess()
        {
            if (_fileService.GetFiles())
            {
                if (await _fileService.ReadCSVAsync())
                    Console.WriteLine("CSV files have been read");
                if (await _fileService.ReadTXTAsync())
                    Console.WriteLine("TXT files have been read");
            }
        }
        public string GetPathFolderA()
        {
            return _fileService.PathFolderA;
        }
    }
}
