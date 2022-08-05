using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Service.Interfaces
{
    public interface IETLService
    {
        Task StartProcess();
        string GetPathFolderA();
        Task SaveMetaLog();
    }
}
