using ETL.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Service.Implementations
{
    public class ETLService: IETLService
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

        public async Task SaveMetaLog()
        {
            if(await _fileService.CreateMetaLogFile())
            {
                Console.WriteLine("meta.log file has been created");
            }
        }
    }
}
