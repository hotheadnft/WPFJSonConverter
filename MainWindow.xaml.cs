using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string _selectedCollection;
        private string _fullPathToJSON;
        private string _fullPathToDB;
        public List<String> filesProcessed = new();
        private readonly List<Tables> _alltables = new();
        public List<JSONFiles> jsonDisplayList = new();
        private readonly List<string> _filteredTables = new();

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
                    collectionName.Content = value;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            add.IsEnabled = false;
            view.IsEnabled = false;
            DataContext = this;
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
            System.Windows.Application.Current.Shutdown();
        }

        private void AddButton_Checked(object sender, RoutedEventArgs e)
        {
            add.IsChecked = false;
            int fileCount;

            //showselected.Visibility = Visibility.Visible;
            //      JsonFileName = DisplayJsonFileBeforeAdding();
            Microsoft.Win32.OpenFileDialog openFile = new()
            {
                Filter = "Json files|*.json",
                Title = "Select file to open",
                Multiselect = true
            };
            var result = openFile.ShowDialog();
            if (result == true)
            {
                lbxFileNameList.IsEnabled = true;
                lbxFileNameList.Visibility = Visibility.Visible;
                fileCount = openFile.FileNames.Length;
                borderfileslist.Visibility = Visibility.Visible;
                Application.Current.MainWindow = this;
                Application.Current.MainWindow.Height = 1420;
                lbxFileNameList.Visibility = Visibility.Visible;
                StreamWriter sw;
                string[] path_parts;
                string[] content = { "" };
                string imagePath, imageFileName;
                string exeDir = Directory.GetCurrentDirectory();
                //read projectmetadata.json to use as template
                var projectTemplate = File.ReadAllLines($"{exeDir}\\projectmetadata.json");
                List<string> filestocheck = new List<string>();
                string jsonFolder = string.Empty;
                string[] possibleFilesProcessed = { "" };
                string fnameOnly = string.Empty;
                string rows = string.Empty;

                StringBuilder pathToCopyJSONFrom = new StringBuilder(); //folder structure is pathToCopyJSONFrom with
                                                                        //orig,images and folder with json files as sub-folders

                jsonFolder = Path.GetDirectoryName(openFile.FileNames[0]);
                path_parts = jsonFolder.Split('\\');

                pathToCopyJSONFrom.Append(path_parts[0] + "\\");
                for (int i = 1; i < path_parts.Length - 1; i++)
                {
                    pathToCopyJSONFrom.Append(path_parts[i] + "\\");
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
                    // lbxFileNameList.Items.Add(Path.GetFileName(filesProcessed[idx]));
                    // lbxFileNameList.SelectedItems.Add(lbxFileNameList.Items[idx]);
                    jsonDisplayList.Add(file);
                }

                lbxFileNameList.ItemsSource = jsonDisplayList;
                foreach (object filetoadd in lbxFileNameList.Items)
                    lbxFileNameList.SelectedItems.Add(filetoadd);

                ///
                ///adding entire folder for now
                ///

                
                var record = new NFT_Maker_format();
                for (int i = 0; i < filesProcessed.Count; i++)
                {
                    fnameOnly = Path.GetFileName(filesProcessed[i]);
                    //get json file to orig folder for safe keeping
                    if (!File.Exists($"{pathToCopyJSONFrom}\\orig\\" + fnameOnly))
                        File.Copy(filesProcessed[i], $"{pathToCopyJSONFrom}\\orig\\" + fnameOnly);

                    //read first json file
                    var NftMakerToConvert = File.ReadAllLines(filesProcessed[i]);
#if !DEBUG
                    //delete and create new json file starting with project template
                    File.Delete(filesProcessed[i]);
#endif
                    StringBuilder sbJsonRecord = new StringBuilder();
                    sw = File.CreateText(filesProcessed[i]);
                    Debug.WriteLine($"Created file {filesProcessed[i]}");
                    //build attributes in two lists as members of record
                    record.parseAttributes(NftMakerToConvert, sw);
                    //write out top half of template
                    for (int template = 0; template < 15; template++)
                    {
                        if (template == 4)
                        {
                            Debug.WriteLine($"        { record.name}");
                            sw.WriteLine($"        { record.name}");
                            // template++;
                            continue;
                        }
                        //replace description in line 8
                        if (template == 7)
                        {
                            Debug.WriteLine($"          { record.description},");
                            sw.WriteLine($"          { record.description},");
                            // template++;
                            continue;
                        }
                        if (template == 10)
                        {
                            Debug.WriteLine($"          { record.name}");
                            sw.WriteLine($"          { record.name}");
                            // template++;
                            continue;
                        }
                        Debug.WriteLine(projectTemplate[template]);
                        sw.Write(projectTemplate[template] + Environment.NewLine);
                    }
                    //walk each list and add trait_type and value as
                    for (int index = 0; index < record.trait_type.Count; index++)
                    {
                        sbJsonRecord.Append("		 ");
                        sbJsonRecord.Append(record.trait_type[index]);
                        sw.Write(sbJsonRecord);
                        Debug.Write(sbJsonRecord);
                        sbJsonRecord.Clear();
                    }

                    sbJsonRecord.Append("    }" + Environment.NewLine + "   }," + Environment.NewLine + "    \"version\": \"1.0\"" + Environment.NewLine + "   }" + Environment.NewLine + "}");
                    Debug.Write(sbJsonRecord.ToString());

                    //Debug.WriteLine("    }" + Environment.NewLine + "   }," + Environment.NewLine + "    \"version\": \"1.0\"" + Environment.NewLine + "   }" + Environment.NewLine + "}");
                    sw.WriteLine("    }" + Environment.NewLine + "   }," + Environment.NewLine + "    \"version\": \"1.0\"" + Environment.NewLine + "   }" + Environment.NewLine + "}");
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
                    Debug.WriteLine(er.Message);
                }

                //filestocheck = FileHelper.GetFilesRecursive("images");
                //   for (int i = 0; i < filestocheck.Count - 1; i++)
                //   { //copy image file for each json file
                //       if (!File.Exists("orig\\" + path_parts[1]))
                //           File.Copy(filestocheck[i], $"{pathToCopyJSONFrom}" + fnameOnly);
                // }
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

            Debug.WriteLine(SelectedCollection + " is the selected collection" + Environment.NewLine);

            add.IsEnabled = true;

            exit.IsEnabled = true;
            //    folderAdd.IsEnabled = true;
        }

        private async void Open_Checked(object sender, RoutedEventArgs e)
        {
            await OpenDB();
            add.IsEnabled = true;
            view.IsEnabled = true;
            showJson();
        }

        private void View_Checked(object sender, RoutedEventArgs e)
        {
            string[] jsonFiles;
            jsonFiles = GetListOfJsonFiles();
        }

        private string[] GetListOfJsonFiles()
        {
            view.IsChecked = false;
            int filecount;
            Microsoft.Win32.OpenFileDialog openFile = new()
            {
                Filter = "Json files|*.json",
                Title = "Select file to open",
                Multiselect = false
            };
            var result = openFile.ShowDialog();
            return openFile.FileNames;
        }

        private void lbxFileNameList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lbxFileNameList.SelectedItem != null)
               Name = (lbxFileNameList.SelectedItem as JSONFiles).Name;
              
        }

        private void selectallfiles_Checked(object sender, RoutedEventArgs e)
        {
            foreach (object filetoadd in lbxFileNameList.Items)
                lbxFileNameList.SelectedItems.Add(filetoadd);
        }
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