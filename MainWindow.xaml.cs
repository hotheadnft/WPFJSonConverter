using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        private readonly List<string> _filteredTables = new();
        public List<ChkboxState> _chkboxStateList;

        private List<ChkboxState> ChkboxStateList
        {
            get
            {
                if (_chkboxStateList == null)
                    _chkboxStateList = new List<ChkboxState>();
                return _chkboxStateList;
            }
            set
            {
                _chkboxStateList = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("ChkboxStateList"));
            }
        }

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
            StreamWriter sw;
            string[] path_parts;
            string exeDir = Directory.GetCurrentDirectory();
            //read projectmetadata.json to use as template
            var projectTemplate = File.ReadAllLines($"{exeDir}\\projectmetadata.json");

            string jsonFolder = string.Empty;
            string[] possibleFilesProcessed = { "" };
            string[] possibleCountProcessed = { "" };

            string fnameOnly = string.Empty;
            lbxFileNameList.Visibility = Visibility.Visible;
            // int rows = 0;
            string rows = string.Empty;
            int filecount;
            add.IsChecked = false;

            //showselected.Visibility = Visibility.Visible;
            //      JsonFileName = DisplayJsonFileBeforeAdding();

            lbxFileNameList.Visibility = Visibility.Visible;
            string[] content = { "" };
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
                
                filecount = openFile.FileNames.Length;
                jsonFolder = Path.GetDirectoryName(openFile.FileNames[0]);
                Directory.SetCurrentDirectory($"{jsonFolder}\\..");
                string currdir = Directory.GetCurrentDirectory();
                path_parts = jsonFolder.Split('\\');


                foreach (string filen in openFile.FileNames)
                {
                    fnameOnly = Path.GetFileName(filen);
                    filesProcessed.Add(fnameOnly);
                }
               
                filesProcessed.Reverse();
                foreach (string fn in filesProcessed)
                {
                    lbxFileNameList.Items.Add(fn);
                }
                ///adding entire folder for now
                var record = new NFT_Maker_format();
                for (int i = 0; i < filesProcessed.Count; i++)
                {
                    fnameOnly= filesProcessed[i];
                    //get json file to orig folder for safe keeping
                    if (!File.Exists($"{currdir}\\orig\\" + fnameOnly))
                        File.Copy(filesProcessed[i], "..\\orig\\" + fnameOnly);

                    //read first json file
                    var NftMakerToConvert = File.ReadAllLines(filesProcessed[i]);

                    //delete and create new json file starting with project template
                    File.Delete(filesProcessed[i]);
                    sw = File.CreateText(filesProcessed[i]);
                    Console.WriteLine($"Created file {filesProcessed[i]}");
                    //build attributes in two lists as members of record
                    record.parseAttributes(NftMakerToConvert, sw);
                }
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
    }

    public class ChkboxState
    {
        private int id;

        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}