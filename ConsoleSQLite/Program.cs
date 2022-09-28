

using SQLiteUTF8CIComparison;
using System.Data.SQLite;
//using System.Data.SQLite;

Console.WriteLine(DateTime.Now + " Opening database...");
 

InitCollation();
using (SQLiteConnection conn = new SQLiteConnection("Data Source=temp.SQLite;"))
{
    
    conn.Open();

    Console.WriteLine(DateTime.Now + " Preparing...");

    CreateTable(conn);
    EnableAsyncWrite(conn);

    Console.WriteLine(DateTime.Now + " Inserting data...");

    InsertSampleData(conn);

    Console.WriteLine(DateTime.Now + " Creating index...");

    CreateIndex(conn);

    Console.WriteLine(DateTime.Now + " Selecting...");

    using (SQLiteCommand cmd = conn.CreateCommand())
    {
        // Sort the data using our index (this must be very quick) 
        // As we sort DESC, Russian words must be the first 
        cmd.CommandText = "select name from testtbl where Lower(name) like Lower('i%')";
        using (SQLiteDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine(reader[0]);
            }
            reader.Close();
        }
    }

    conn.Close();
}
Console.WriteLine(DateTime.Now + " Done.");
Console.ReadKey();



/// <summary>
/// Creates an index on testtbl.Name 
/// </summary>
/// <param name="conn">Connection to SQLite</param>
static void CreateIndex(SQLiteConnection conn)
{
    using (SQLiteCommand cmd = conn.CreateCommand())
    {
        cmd.CommandText = "CREATE INDEX IDX_testtbl_Name ON testtbl (Name COLLATE NOCASE)";
        cmd.ExecuteNonQuery();
    }
}

/// <summary>
/// Enables asynchronous mode to perform faster INSERT operations 
/// </summary>
/// <param name="conn">Connection to SQLite</param>
static void EnableAsyncWrite(SQLiteConnection conn)
{
    using (SQLiteCommand cmd = conn.CreateCommand())
    {
       // cmd.CommandText = "PRAGMA synchronous = 0;PRAGMA cipher_compatibility = 3;PRAGMA encoding=\"UTF-8\";";
        //cmd.CommandText = "PRAGMA  = 0;";
        //Database.ExecuteSqlRaw("PRAGMA case_sensitive_like = 0;PRAGMA cipher_compatibility = 3;");
        //Database.ExecuteSqlRaw("PRAGMA encoding=\"UTF-8\";");
        //cmd.ExecuteNonQuery();
    }
}

/// <summary>
/// Creates table "testtbl"
/// Table fields: 
/// ID   - ID 
/// Name - a sample string value (on this we'll test our collation)
/// Val - some random value 
/// </summary>
/// <param name="conn">Connection to SQLite</param>
static void CreateTable(SQLiteConnection conn)
{
    using (SQLiteCommand cmd = conn.CreateCommand())
    {
        cmd.CommandText = @"drop TABLE testtbl; CREATE TABLE testtbl (
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    Name TEXT NOT NULL COLLATE NOCASE, 
    Val REAL);";
        cmd.ExecuteNonQuery();
    }
}

/// <summary>
/// Inserts large amount of sample rows into "testtbl" table 
/// </summary>
/// <param name="conn">Connection to SQLite</param>
static void InsertSampleData(SQLiteConnection conn)
{
    using (SQLiteTransaction txn = conn.BeginTransaction())
    {
        InsertRow(conn, "İSTANBUL", 1);
        InsertRow(conn, "ısparta", 1);
        InsertRow(conn, "çarşamba", 1);
        InsertRow(conn, "İlk Başlık", 1);
        InsertRow(conn, "istikamet", 1);
        InsertRow(conn, "Буква", 1); // Russian word from uppercase letter 
        InsertRow(conn, "буква", 1); // Russian word from lowercase letter
        InsertRow(conn, "Тест", 1); // Another Russian word from uppercase letter 
        InsertRow(conn, "тест", 1); // Another Russian word from lowercase letter 

        // Uncomment this if you want to insert sample data to increase volume and check that inedex is really applied 
        /*for (int i = 0; i < 1000000; i++) {
            InsertRow(conn, "Foo" + i, i);
            InsertRow(conn, "foo" + i, i);
            InsertRow(conn, "Bar" + i, i);
            InsertRow(conn, "bar" + i, i);
            if (i % 1000 == 0) {
                Console.Write(".");
            }
        }*/
        txn.Commit();
    }

    Console.WriteLine();
}

/// <summary>
/// Inserts a row into the database
/// </summary>
/// <param name="conn">Connection to SQLite</param>
/// <param name="name">"Name" field</param>
/// <param name="val">"Val" field</param>
static void InsertRow(SQLiteConnection conn, string name, double val)
{
    using (SQLiteCommand cmd = conn.CreateCommand())
    {
        cmd.CommandText = "INSERT INTO testtbl (Name, Val) VALUES (@1, @2)";
        cmd.Parameters.AddWithValue("@1", name);
        cmd.Parameters.AddWithValue("@2", val);
        cmd.ExecuteNonQuery();
    }
}

/// <summary>
/// Initializes the collation UTF8CI 
/// </summary>
static void InitCollation()
{
    SQLiteFunction.RegisterFunction(typeof(SQLiteCaseInsensitiveCollation));
}