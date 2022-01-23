using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfHashlipsJSONConverter
{
    public partial class MainWindow
    {
        public async Task<List<string>> GetTables(string fullPathDB)
        {
            using SQLiteConnection connection = new($"Data Source={fullPathDB}");
            connection.Open();
            _fullPathToDB = connection.FileName;
            FullPathToDB = fullPathDB;
            List<string> rtables = new();
            DataTable dt = await connection.GetSchemaAsync("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                rtables.Add(tablename);
            }

            return rtables;
        }

        public async Task<int> OpenDB()
        {
            string currdir = string.Empty;
            int filecount = 0;
            string fnameOnly = string.Empty;
            List<String> filesProcessed = new List<String>();

            Microsoft.Win32.OpenFileDialog openFile = new()
            {
                Filter = "Database|*.DB",
                Title = "Select file to open",
                Multiselect = true
            };
            var result = openFile.ShowDialog();
            if (result == true)
            {
                currdir = Directory.GetCurrentDirectory();
                filecount = openFile.FileNames.Length;
                fileNameToDisplay.IsEnabled = true;
                fileNameToDisplay.Content = Path.GetFileName(openFile.FileName);
                fileNameToDisplay.Visibility = System.Windows.Visibility.Visible;

                FullPathToDB = openFile.FileName;
                filesProcessed = await GetTables(FullPathToDB);

                foreach (string table in filesProcessed)
                {
                    if (table.CompareTo("sqlite_sequence") == 0)
                        continue;
                    Tables DBTables = new();
                    DBTables.Name = table;
                    _alltables.Add(DBTables);
                    _filteredTables.Add(DBTables.Name);
                }
                startInstructions.Content = @"Pick collection type from drop down";
                tableList.Visibility = System.Windows.Visibility.Visible;
                tableList.ItemsSource = _alltables;
            }

            return 0;
        }

        public void showJsonTemplate()
        {
            JsonSerializerOptions options;
            string json = string.Empty;
            string jsonDisplay = String.Empty;
            switch (SelectedCollection)
            {
                case "InImage":
                    InImage image = new();
                    options = new JsonSerializerOptions { WriteIndented = true };
                    json = System.Text.Json.JsonSerializer.Serialize(image);
                    break;

                case "Eyeball9":
                    Eyeball9 eyeball = new();
                    options = new JsonSerializerOptions { WriteIndented = true };
                    json = System.Text.Json.JsonSerializer.Serialize(eyeball, options);
                    break;

                default:
                    break;
            }

            //templateDisplay.DataContext = this;
            templateDisplay.Text = json;
            templateDisplay.Visibility = System.Windows.Visibility.Visible;

            Debug.WriteLine(json);
            open.IsChecked = false;
        }
    }
}