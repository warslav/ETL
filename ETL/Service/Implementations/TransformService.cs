using ETL.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Service.Implementations
{
    public class TransformService
    {
        public string TransactionsGroupByCityAndService(List<TransactionDTO> transactions)
        {
            var linqQuery = transactions
            .GroupBy(x => x.Address.Split(",").First())
            .Select(g => new
            {
                city = g.Key,
                Services = g.Select(i => new
                {
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    Address = i.Address,
                    Payment = i.Payment,
                    Date = i.Date,
                    AccountNumber = i.AccountNumber,
                    Service = i.Service
                }).ToList()
                    .Where(x => x.Address.StartsWith(g.Key))
                    .Distinct()
                    .GroupBy(s => s.Service)
                    .Select(s => new
                    {
                        name = s.Key,
                        payers = s.Select(i => new
                        {
                            name = string.Join(" ", new[] { i.FirstName, i.LastName }),
                            payment = i.Payment,
                            date = i.Date,
                            account_number = i.AccountNumber
                        }).ToList(),
                        total = s.Sum(x => x.Payment)
                    }).ToList(),
                total = g.Sum(x => x.Payment)
            }).ToList();
            var json = JsonConvert.SerializeObject(linqQuery);
            return json;
        }
    }
}
