using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        // Input and output directory paths
        string inputDirectory = @"/Users/dhruvbaliyan/Documents/aadhaar_csv/demo"; // Replace with your input directory
        string outputDirectory = @"/Users/dhruvbaliyan/Documents/aadhaar_csv/demo/result"; // Replace with your output directory

        // Process each CSV file in the input directory
        string[] csvFiles = Directory.GetFiles(inputDirectory, "*.csv");
        foreach (string csvFile in csvFiles)
        {
            ProcessCsvFile(csvFile, outputDirectory);
        }
        stopwatch.Stop();
        double executionSeconds = stopwatch.Elapsed.TotalSeconds;
        Console.WriteLine($"Processing completed in {executionSeconds} seconds");
    }

    static void ProcessCsvFile(string inputFile, string outputDirectory)
    {
        // Configure CSV reader and writer
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

        // Create a unique output file name based on the input file name
        string outputFileName = Path.GetFileNameWithoutExtension(inputFile) + "_hashed.csv";
        string outputPath = Path.Combine(outputDirectory, outputFileName);

        using (var reader = new StreamReader(inputFile))
        using (var csv = new CsvReader(reader, csvConfig))
        using (var writer = new StreamWriter(outputPath))
        using (var csvWriter = new CsvWriter(writer, csvConfig))
        {
            // Write CSV header if needed
            // csvWriter.WriteHeader<MyCsvRecord>();
            // csvWriter.NextRecord();

            while (csv.Read())
            {
                var record = new MyCsvRecord
                {
                    UserId = csv.GetField<int>(0), // Assuming UserId is in the first column
                    AadhaarNo = csv.GetField<string>(1), // Assuming AadhaarNo is in the second column
                };

                // Compute SHA-256 hash of AadhaarNo
                string hashedAadhaarNo = ComputeSha256Hash(record.AadhaarNo);

                // Create a new record with UserId and hashedAadhaarNo
                var newRecord = new MyCsvRecord
                {
                    UserId = record.UserId,
                    AadhaarNo = hashedAadhaarNo
                };

                // Write the new record to the output CSV file
                csvWriter.WriteRecord(newRecord);
                csvWriter.NextRecord();
            }
        }

        Console.WriteLine($"Processed: {inputFile}");
    }

    static string ComputeSha256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}

// Define a class to represent the CSV records
class MyCsvRecord
{
    public int UserId { get; set; }
    public string AadhaarNo { get; set; }
}
