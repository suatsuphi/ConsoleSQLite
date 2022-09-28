
using System.Data.SQLite;
using System.Globalization;
using System.Xml.Linq;

namespace SQLiteUTF8CIComparison
{
    /// <summary>
    /// This function adds case-insensitive sort feature to SQLite engine 
    /// To initialize, use SQLiteFunction.RegisterFunction() before all connections are open 
    /// </summary>
    [SQLiteFunction(FuncType = FunctionType.Collation, Name = "UTF8CI")]
    public class SQLiteCaseInsensitiveCollation : SQLiteFunction
    {
        /// <summary>
        /// CultureInfo for comparing strings in case insensitive manner 
        /// </summary>
        private static readonly CultureInfo _cultureInfo = CultureInfo.CreateSpecificCulture("tr-TR");

        /// <summary>
        /// Does case-insensitive comparison using _cultureInfo 
        /// </summary>
        /// <param name="x">Left string</param>
        /// <param name="y">Right string</param>
        /// <returns>The result of a comparison</returns>
        public override int Compare(string x, string y)
        {
            return string.Compare(x, y, _cultureInfo, CompareOptions.IgnoreCase);
        }
    }

    [SQLiteFunction(Name = "UPPER", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class Upper : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return Convert.ToString(args[0]).ToUpper();
        }
    }

    [SQLiteFunction(Name = "LOWER", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class Lower : SQLiteFunction
    {
        public override object Invoke(object[] args)
        { 
            return Convert.ToString(args[0]).ToLower();
        }
    }

}