using CsvHelper;
using CsvHelper.Configuration;
using ETL.DTO;
using FileHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETL.Service.Implementations
{
    public class FileService
    {
        MetaFiles _metaFiles;
        ETLSettings _etlSettings;
        TransformService _transformService;
        public string PathFolderA { get; private set; }
        public FileService(IOptions<ETLSettings> etlSettings, MetaFiles metaFiles, TransformService transformService)
        {
            _etlSettings = etlSettings.Value;
            PathFolderA = _etlSettings.FolderA;
            CheckDirectories();
            _metaFiles = metaFiles;
            _transformService = transformService;
        }
        public void CheckDirectories()
        {
            if (!Directory.Exists(_etlSettings.FolderA))
                Directory.CreateDirectory(_etlSettings.FolderA);
            if (!Directory.Exists(_etlSettings.FolderB))
                Directory.CreateDirectory(_etlSettings.FolderB);
        }
        public bool GetFiles()
        {
            try
            {
                var filesAll = Directory.GetFiles(_etlSettings.FolderA);
                if (!filesAll.Any())
                    return false;
                foreach (var item in filesAll)
                {
                    string fileExtensions = Path.GetExtension(item);
                    switch (fileExtensions)
                    {
                        case ".csv":
                            _metaFiles.CSVFiles.Add(item);
                            break;
                        case ".txt":
                            _metaFiles.TXTFiles.Add(item);
                            break;
                        default:
                            _metaFiles.InvalidFiles.Add(item);
                            break;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ReadCSVAsync()
        {
            try
            {
                if (!_metaFiles.CSVFiles.Any())
                    return false;
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    Encoding = Encoding.UTF8,
                    HasHeaderRecord = true,
                };
                foreach (var item in _metaFiles.CSVFiles)
                {
                    List<TransactionDTO> transactionDTOs = new List<TransactionDTO>();
                    using (StreamReader streamReader = new StreamReader(item))
                    {
                        using (CsvReader csvReader = new CsvReader(streamReader, config))
                        {
                            while (csvReader.Read())
                            {
                                try
                                {
                                    var transaction = csvReader.GetRecord<TransactionDTO>();
                                    transactionDTOs.Add(transaction);
                                    _metaFiles.ParsedLines.Add(csvReader.Parser.RawRecord);
                                }
                                catch (Exception)
                                {
                                    _metaFiles.FoundErrors.Add(csvReader.Parser.RawRecord);
                                }
                            }
                        }
                    }
                    if (!transactionDTOs.Any())
                    {
                        _metaFiles.InvalidFiles.Add(item);
                        continue;
                    }
                    _metaFiles.ParsedFiles.Add(item);
                    var json = _transformService.TransactionsGroupByCityAndService(transactionDTOs);
                    if (await CreateOutputFile(json))
                        Console.WriteLine("CSV File create");
                    File.Delete(item);
                }
                _metaFiles.CSVFiles.Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public async Task<bool> ReadTXTAsync()
        {
            try
            {
                if (!_metaFiles.TXTFiles.Any())
                    return false;
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    Encoding = Encoding.UTF8,
                    HasHeaderRecord = true,

                };
                foreach (var item in _metaFiles.TXTFiles)
                {
                    List<TransactionDTO> transactionDTOs = new List<TransactionDTO>();
                    using (StreamReader streamReader = new StreamReader(item))
                    {
                        using (CsvReader csvReader = new CsvReader(streamReader, config))
                        {
                            while (csvReader.Read())
                            {
                                try
                                {
                                    var transaction = new TransactionDTO
                                    {
                                        FirstName = csvReader.GetField(0).Trim(),
                                        LastName = csvReader.GetField(1).Trim(),
                                        Address = (csvReader.GetField(2) + "," + csvReader.GetField(3) + "," + csvReader.GetField(4)).Trim(new char[] { '\"', '“', ' ', '”' }),
                                        Payment = csvReader.GetField<decimal>(5, new CustomDecimalConverter()),
                                        Date = csvReader.GetField<DateTime>(6, new CustomDateTimeConverter()),
                                        AccountNumber = csvReader.GetField<long>(7, new CustomLongConverter()),
                                        Service = csvReader.GetField(8).Trim()
                                    };
                                    transactionDTOs.Add(transaction);
                                    _metaFiles.ParsedLines.Add(csvReader.Parser.RawRecord);
                                }
                                catch (Exception)
                                {
                                    _metaFiles.FoundErrors.Add(csvReader.Parser.RawRecord);
                                }
                            }
                        }
                    }
                    if (!transactionDTOs.Any())
                    {
                        _metaFiles.InvalidFiles.Add(item);
                        continue;
                    }
                    _metaFiles.ParsedFiles.Add(item);
                    var json = _transformService.TransactionsGroupByCityAndService(transactionDTOs);
                    if(await CreateOutputFile(json))
                        Console.WriteLine("TXT File create");
                    File.Delete(item);
                }
                _metaFiles.TXTFiles.Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> CreateOutputFile(string json) {
            var dateFolder = Path.Combine(_etlSettings.FolderB, DateTime.Now.ToShortDateString());
            if (!Directory.Exists(dateFolder))
                Directory.CreateDirectory(dateFolder);
            await File.WriteAllLinesAsync(Path.Combine(dateFolder, $"output_{_metaFiles.ParsedFiles.Count}.json"), new string[] { json });
            return true;
        }
    }
}
