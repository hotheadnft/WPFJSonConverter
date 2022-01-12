using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
        private readonly List<Tables> _alltables = new();
        private readonly List<string> _filteredTables = new();
        private readonly List<string> _collectionTables = new();

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
            string currdir = string.Empty;
            string[] possibleFilesProcessed = { "" };
            string[] possibleCountProcessed = { "" };
            string countProcessed;
            int tempCountProcessed = 0;
            string fnameOnly = string.Empty;
            List<String> filesProcessed = new List<String>();
            // int rows = 0;
            string rows = string.Empty;
            int filecount;
            add.IsChecked = false;

            //showselected.Visibility = Visibility.Visible;
            //      JsonFileName = DisplayJsonFileBeforeAdding();

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
                //lblnftFile.IsEnabled = true;
                //lblnftFile.Visibility = Visibility.Visible;

                currdir = Directory.GetCurrentDirectory();
                filecount = openFile.FileNames.Length;
            }
        }

        private void exit_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
        }

        private void tableList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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

        private async void open_Checked(object sender, RoutedEventArgs e)
        {
            OpenDB();
            add.IsEnabled = true;
            view.IsEnabled = true;
            showJson();
        }
    }
}