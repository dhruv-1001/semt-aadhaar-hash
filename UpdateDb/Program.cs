using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

class Program
{
    static void Main()
    {

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        string[] csvFiles = Directory.GetFiles("/Users/dhruvbaliyan/Documents/aadhaar_csv/demo/result", "*.csv");
        
        string serverName = "192.168.1.40";
        string databaseName = "aadhaar";
        string username = "SA";
        string password = "password";
        string connectionString = $"Server={serverName},1433;Database={databaseName};User Id={username};Password={password};";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            foreach (string csvFilePath in csvFiles)
            {
                // Load the CSV file into a DataTable
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("user_id", typeof(int));
                dataTable.Columns.Add("hashed_aadhaar", typeof(string));

                using (StreamReader reader = new StreamReader(csvFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        dataTable.Rows.Add(Convert.ToInt32(parts[0]), parts[1]);
                    }
                }
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "DemoUserAadhaar";
                    bulkCopy.ColumnMappings.Add("user_id", "user_id");
                    bulkCopy.ColumnMappings.Add("hashed_aadhaar", "hashed_aadhaar");

                    // Set the batch size to update 10000 rows at once
                    bulkCopy.BatchSize = 10000;

                    // Set the timeout (adjust as needed)
                    bulkCopy.BulkCopyTimeout = 600; // 10 minutes

                    // Perform the bulk copy operation
                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }
        stopwatch.Stop();
        double executionSeconds = stopwatch.Elapsed.TotalSeconds;
        Console.WriteLine($"Processing completed in {executionSeconds} seconds");
    }
}

public class CSVRecord
{
    public int user_id { get; set; }
    public string hashed_aadhaar { get; set; }
}