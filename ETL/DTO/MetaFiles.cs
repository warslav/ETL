using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.DTO
{
    public class MetaFiles
    {
        public ConcurrentBag<string> TXTFiles { get; set; }
        public ConcurrentBag<string> CSVFiles { get; set; }
        public ConcurrentBag<string> InvalidFiles { get; set; }
        public ConcurrentBag<string> ParsedFiles { get; set; }
        public ConcurrentBag<string> ParsedLines { get; set; }
        public ConcurrentBag<string> FoundErrors { get; set; }

        public MetaFiles()
        {
            TXTFiles = new ConcurrentBag<string>();
            CSVFiles = new ConcurrentBag<string>();
            InvalidFiles = new ConcurrentBag<string>();
            ParsedFiles = new ConcurrentBag<string>();
            ParsedLines = new ConcurrentBag<string>();
            FoundErrors = new ConcurrentBag<string>();
        }
    }
}
