using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.DTO
{
    public class TransactionDTO
    {
        [Index(0)]
        public string FirstName { get; set; }
        [Index(1)]
        public string LastName { get; set; }
        [Index(2)]
        public string Address { get; set; }
        [Index(3)]
        [TypeConverter(typeof(CustomDecimalConverter))]
        public decimal Payment { get; set; }
        [Index(4)]
        [TypeConverter(typeof(CustomDateTimeConverter))]
        public DateTime Date { get; set; }
        [Index(5)]
        [TypeConverter(typeof(CustomLongConverter))]
        public long AccountNumber { get; set; }
        [Index(6)]
        public string Service { get; set; }
    }

    public class CustomDecimalConverter : DecimalConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (decimal.TryParse(text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            else
            {
                throw new Exception($"CustomDecimalConverter: can`t convert {text} to decimal");
            }
        }
    }

    public class CustomDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            DateTime val;
            ;
            if(DateTime.TryParseExact(text.Trim(), "yyyy-dd-MM", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out val))
            {
                return val;
            }
            else
            {
                throw new Exception($"CustomDateTimeConverter: can`t convert {text} to DateTime");
            }
        }
    }

    public class CustomLongConverter : DecimalConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (long.TryParse(text.Trim(), out var result))
            {
                return result;
            }
            else
            {
                throw new Exception($"CustomLongConverter: can`t convert {text} to long");
            }
        }
    }
}
