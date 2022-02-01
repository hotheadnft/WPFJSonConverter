using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WpfHashlipsJSONConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string _selectedCollection;
        public string _fullPathToJSON;
        public string _fullPathToDB;
        public List<String> filesProcessed = new();
        public readonly List<Tables> _alltables = new();
        public List<JSONFiles> jsonDisplayList = new();
        public readonly List<string> _filteredTables = new();
        public string jsonText;

        public event PropertyChangedEventHandler WhichPropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (WhichPropertyChanged != null)
            {
                WhichPropertyChanged(this, e);
            }
        }

        public string FullPathToDB
        {
            get => _fullPathToDB;
            set
            {
                if (value != null)
                {
                    _fullPathToDB = value;
                    NotifyPropertyChanged(_fullPathToDB);
                    fileNameToDisplay.Content = _fullPathToDB;                   
                }
            }
        }

        public string FullPathToJSON { get => _fullPathToJSON; set => _fullPathToJSON = value; }

        public string SelectedCollection
        {
            get => _selectedCollection;
            set
            {
                if (value != null)
                {
                    _selectedCollection = value;
                    NotifyPropertyChanged(_selectedCollection);
                    collectionName.Content = $"Current Collection: {value}";                   
                    ShowJsonTemplate();
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            add.IsEnabled = false;
            view.IsEnabled = true;
            DataContext = this;
            txtblkFileContent.Visibility = Visibility.Hidden;
            templateDisplay.Visibility = Visibility.Hidden;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ButtonMinimizeClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void WindowsStateButtonClick(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                System.Windows.Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                System.Windows.Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }

        private void ButtonExitClick(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine($"Exit directory is: {Directory.GetCurrentDirectory()}");
            if(FullPathToDB != null)
                Directory.SetCurrentDirectory(Path.GetDirectoryName(FullPathToDB));
            System.Windows.Application.Current.Shutdown();
        }

        private async void AddButton_Checked(object sender, RoutedEventArgs e)
        {
            add.IsChecked = false;
            int fileCount;

            Microsoft.Win32.OpenFileDialog openFile = new()
            {
                Filter = "Json files|*.json",
                Title = "Select file to open",
                Multiselect = true
            };
            var result = openFile.ShowDialog();
            if (result == true)
            {
                txtblkFileContent.Visibility = Visibility.Visible;
                fileCount = openFile.FileNames.Length;     
                txtblkFileContent.Text = "Converting...";

                StreamWriter sw;
                string?[] path_parts;
                string[] content = { "" };
                string imagePath, imageFileName;
                string exeDir = Directory.GetCurrentDirectory();
                //read projectmetadata.json to use as template
                var projectTemplate = await File.ReadAllLinesAsync($"{exeDir}\\projectmetadata.json");
                List<string> filestocheck = new List<string>();
                string? jsonFolder = string.Empty;
                string[] possibleFilesProcessed = { "" };
                string fnameOnly = string.Empty;
                string rows = string.Empty;

                StringBuilder pathToCopyJSONFrom = new StringBuilder(); //folder structure is pathToCopyJSONFrom with
                                          
                //orig,images and folder with json files as sub-folders

                jsonFolder = Path.GetDirectoryName(openFile.FileNames[0]);
                if (jsonFolder != null)
                {
                    path_parts = jsonFolder.Split('\\');

                    pathToCopyJSONFrom.Append(path_parts[0] + "\\");
                    for (int i = 1; i < path_parts.Length - 1; i++)
                    {
                        pathToCopyJSONFrom.Append(path_parts[i] + "\\");
                    }
                }
                Directory.SetCurrentDirectory($"{pathToCopyJSONFrom}");
                Directory.CreateDirectory("orig");

                foreach (string filen in openFile.FileNames)
                {
                    filesProcessed.Add(filen);
                }

                filesProcessed.Reverse();

                for (int idx = 0; idx < fileCount; idx++)
                {
                    JSONFiles file = new JSONFiles();
                    file.Name = Path.GetFileName(filesProcessed[idx]);
                    file.IsSelected = true;
                    jsonDisplayList.Add(file);
                }

                var record = new NFT_Maker_format();
                for (int i = 0; i < filesProcessed.Count; i++)
                {
                    fnameOnly = Path.GetFileName(filesProcessed[i]);
                    //get json file to orig folder for safe keeping
                    if (!File.Exists($"{pathToCopyJSONFrom}\\orig\\" + fnameOnly))
                        File.Copy(filesProcessed[i], $"{pathToCopyJSONFrom}\\orig\\" + fnameOnly);

                    //read first json file
                    var NftMakerToConvert = await File.ReadAllLinesAsync(filesProcessed[i]);
#if !DEBUG
                    //delete and create new json file starting with project template
                    File.Delete(filesProcessed[i]);
#endif
                    StringBuilder sbJsonRecord = new StringBuilder();
                    sw = File.CreateText(filesProcessed[i]);
                    //build attributes in two lists as members of record
                    record.parseAttributes(NftMakerToConvert);
                    //write out top half of template
                    for (int template = 0; template < 15; template++)
                    {
                        if (template == 4)
                        {
                           // ////Debug.WriteLine($"        { record.name}");
                            await sw.WriteLineAsync($"        { record.name}");
                            // template++;
                            continue;
                        }
                        //replace description in line 8
                        if (template == 7)
                        {
                          //  ////Debug.WriteLine($"          { record.description},");
                            await sw.WriteLineAsync($"          { record.description},");
                            // template++;
                            continue;
                        }
                        if (template == 10)
                        {
                           // ////Debug.WriteLine($"          { record.name}");
                            await sw.WriteLineAsync($"          { record.name}");
                            // template++;
                            continue;
                        }
                      // ////Debug.WriteLine(projectTemplate[template]);
                       await sw.WriteAsync(projectTemplate[template] + Environment.NewLine);
                    }
                    //walk each list and add trait_type and value as
                    for (int index = 0; index < record.trait_type.Count; index++)
                    {
                        sbJsonRecord.Append("		 ");
                        sbJsonRecord.Append(record.trait_type[index]);
                       await sw.WriteAsync(sbJsonRecord);
                        //Debug.Write(sbJsonRecord);
                        sbJsonRecord.Clear();
                    }

                    sbJsonRecord.Append("    }" + Environment.NewLine + "   }," + Environment.NewLine + "    \"version\": \"1.0\"" + Environment.NewLine + "   }" + Environment.NewLine + "}");
                  //  //Debug.Write(sbJsonRecord.ToString());
                 
                    //////Debug.WriteLine("    }" + Environment.NewLine + "   }," + Environment.NewLine + "    \"version\": \"1.0\"" + Environment.NewLine + "   }" + Environment.NewLine + "}");
                    await sw.WriteLineAsync("    }" + Environment.NewLine + "   }," + Environment.NewLine + "    \"version\": \"1.0\"" + Environment.NewLine + "   }" + Environment.NewLine + "}");
                    sbJsonRecord.Clear();
                    sw.Close();
                }
                StringBuilder imagesPathToCopyFrom = new StringBuilder();
                possibleFilesProcessed = Directory.GetFiles($"{pathToCopyJSONFrom}\\images");

                try
                {
                    foreach (var item in possibleFilesProcessed)
                    {
                        imageFileName = Path.GetFileName(item);
                        imagesPathToCopyFrom.Append(jsonFolder);
                        imagesPathToCopyFrom.Append("\\");
                        imagesPathToCopyFrom.Append(imageFileName);
                        imagesPathToCopyFrom.Replace(".png", ".json");
                        if (File.Exists(imagesPathToCopyFrom.ToString()))
                        {
                            imagesPathToCopyFrom.Replace(".json", ".png");
                            File.Copy(item, $"{imagesPathToCopyFrom}");
                        }
                        imagesPathToCopyFrom.Clear();
                        // filestocheck.Add(item);
                    }
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message);
                }

                //filestocheck = FileHelper.GetFilesRecursive("images");
                //   for (int i = 0; i < filestocheck.Count - 1; i++)
                //   { //copy image file for each json file
                //       if (!File.Exists("orig\\" + path_parts[1]))
                //           File.Copy(filestocheck[i], $"{pathToCopyJSONFrom}" + fnameOnly);
                // }
                txtblkFileContent.Text = "Finished!";
                Directory.SetCurrentDirectory(Path.GetDirectoryName(FullPathToDB));
            }
        }

        private void TableList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            tableList.IsEnabled = true;
            tableList.DataContext = this;
            Tables tables = new();
            tables = (Tables)tableList.SelectedValue;

            collectionName.Visibility = Visibility.Visible;
            collectionName.IsEnabled = true;
            SelectedCollection = tables.Name;

            ////Debug.WriteLine(SelectedCollection + " is the selected collection" + Environment.NewLine);

            add.IsEnabled = true;

            exit.IsEnabled = true;
            //    folderAdd.IsEnabled = true;
        }

        private async void Open_Checked(object sender, RoutedEventArgs e)
        {
            OpenDB();
            add.IsEnabled = true;
            view.IsEnabled = true;
        }

        private async void View_Checked(object sender, RoutedEventArgs e)
        {
            string[] jsonFiles;

            //  List<string> viewFile = new List<string>();
            jsonFiles = GetListOfJsonFiles();
            if (jsonFiles.Length > 0)
            {
                foreach (string jsonFile in jsonFiles)
                    filesProcessed.Add(jsonFile);
              
                jsonText = await File.ReadAllTextAsync(filesProcessed[0].ToString());


                Application.Current.MainWindow = this;
                Application.Current.MainWindow.Height = 700;
                Application.Current.MainWindow.Width = 1550;
                txtblkFileContent.Visibility = Visibility.Visible;

                txtblkFileContent.Text = jsonText;
            }
            Directory.SetCurrentDirectory(Path.GetDirectoryName(FullPathToDB));
        }
        public List<string> GetTables(string fullPathDB)
        {
            using SQLiteConnection connection = new($"Data Source={fullPathDB}");
            connection.Open();

            //  _fullPathToDB = connection.FileName;
            //  FullPathToDB = fullPathDB;
            List<string> rtables = new();
            DataTable dt = connection.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                rtables.Add(tablename);
            }

            return rtables;
        }

        public void OpenDB()
        {
            // string currdir = string.Empty;
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
                // currdir = Directory.GetCurrentDirectory();
                filecount = openFile.FileNames.Length;
                fileNameToDisplay.IsEnabled = true;
                fileNameToDisplay.Content = Path.GetFileName(openFile.FileName);
                fileNameToDisplay.Visibility = System.Windows.Visibility.Visible;

                FullPathToDB = openFile.FileName;
                filesProcessed = GetTables(FullPathToDB);

                foreach (string table in filesProcessed)
                {
                    if (table.CompareTo("sqlite_sequence") == 0)
                        continue;
                    Tables DBTables = new();
                    DBTables.Name = table;
                    _alltables.Add(DBTables);
                    _filteredTables.Add(DBTables.Name);
                }

                tableList.Visibility = System.Windows.Visibility.Visible;
                tableList.ItemsSource = _alltables;
            }
        }
        private string[] GetListOfJsonFiles()
        {
            view.IsChecked = false;

            Microsoft.Win32.OpenFileDialog openFile = new()
            {
                Filter = "Json files|*.json",
                Title = "Select file to open",
                Multiselect = false
            };
            var result = openFile.ShowDialog();
            return openFile.FileNames;
        }

       // public string GetProjectMetadataFile()
       // {
       //     Microsoft.Win32.OpenFileDialog openFile = new()
       //     {
       //         Filter = "Json files|projectmetadata.json",
       //         Title = "PROJECTMETADAT.JSON",
       //         Multiselect = false
       //     };
       //     var result = openFile.ShowDialog();
       //
       //     List<string> list = new List<string>(openFile.FileNames);
       //     return list[0];
       // }
    }

    public class JSONFiles
    {
        private string _name;
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }

        public JSONFiles()
        {
        }
    }
}