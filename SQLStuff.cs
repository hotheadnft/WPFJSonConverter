using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHashlipsJSONConverter
{
    public class sqlstuff :MainWindow
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

                tableList.Visibility = System.Windows.Visibility.Visible;
                tableList.ItemsSource = _alltables;
            }

            return 0;
        }

        public async Task<object> AddRowFromListAsync(List<string> nftsToAdd, string SelectedCollection, string pathToDB, List<string> namesAdded)
        {


            /// Generic parts outside of loop
            /// 
            //dbfile, connection

            /// Depends on NFT outside of loop
            /// 
            // all traits specific to collection

            /// Generic parts inside of loop
            /// 
            // rows,trans, command,dcode,web,twitter;
            /// Depends on nft inside loop
            // eyeball9, eyeball, InImage, inimage, etc

            string background, eyeball, eyecolor, iris, shine, bottom_lid, top_lid, dcode, collectionname, twitter, web, name, description;
            int price, rows = 0;
          
            Directory.SetCurrentDirectory(pathToDB);
            string dbfile = "URI=file:NFTDB.db";
            SQLiteConnection connection = new(dbfile);
            connection.Open();
            //  var trans = connection.BeginTransaction();
            string addCollection = "insert into Eyeball9(id,name,description,eyeball,eyecolor,iris,shine,bottom_lid,top_lid,background,dcode,twitter,web,price,sold,max_copies,total_minted,collectionname)" +
                              "VALUES (@id,@name,@description,@eyeball,@eyecolor,@iris,@shine,@bottom_lid,@top_lid,@background,@dcode,@twitter,@web,@price,@sold,@max_copies,@total_minted,@collectionname);";

            for (int i = 0; i < nftsToAdd.Count; i++)
            {
                // currdir = Directory.GetCurrentDirectory();
                //connection.Open();
                var trans = connection.BeginTransaction();
                SQLiteCommand command = new(addCollection, connection);

                Eyeball9 eyeballToAdd = new();
                await eyeballToAdd.CollectionBuildRecord(nftsToAdd[i]);

                price = eyeballToAdd.Price;
                description = eyeballToAdd.Description;
                int sold, max_copies, total_minted;
                sold = max_copies = total_minted = 0;
                dcode = eyeballToAdd.Dcode;
                twitter = eyeballToAdd.Twitter;
                web = eyeballToAdd.Web;
                eyeball = eyeballToAdd.Eyeball;
                background = eyeballToAdd.Background;
                eyecolor = eyeballToAdd.Eyecolor;
                iris = eyeballToAdd.Iris;
                shine = eyeballToAdd.Shine;
                bottom_lid = eyeballToAdd.Bottom_lid;
                top_lid = eyeballToAdd.Top_lid;
                collectionname = SelectedCollection;
                name = eyeballToAdd.Name;

                //Directory.SetCurrentDirectory(currdir);

                command.Parameters.AddWithValue("@id", null);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@eyeball", eyeball);
                command.Parameters.AddWithValue("@eyecolor", eyecolor);
                command.Parameters.AddWithValue("@iris", iris);
                command.Parameters.AddWithValue("@shine", shine);
                command.Parameters.AddWithValue("@bottom_lid", bottom_lid);
                command.Parameters.AddWithValue("@top_lid", top_lid);
                command.Parameters.AddWithValue("@background", background);
                command.Parameters.AddWithValue("@dcode", dcode);
                command.Parameters.AddWithValue("@twitter", twitter);
                command.Parameters.AddWithValue("@web", web);

                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@sold", sold);
                command.Parameters.AddWithValue("@max_copies", max_copies);
                command.Parameters.AddWithValue("@total_minted", total_minted);
                command.Parameters.AddWithValue("@collectionname", collectionname);
                try
                {
                    namesAdded.Add(Path.GetFileName(nftsToAdd[i]));
                    //rows +=
                    rows += await command.ExecuteNonQueryAsync();
                    trans.Commit();
                    Eyeball9.recordCount++;
                }
                catch (SQLiteException sqc)
                {
                    namesAdded.RemoveAt(i);
                    string ecode = sqc.ErrorCode.ToString();
                    string rcode = sqc.ResultCode.ToString();
                    if ((rcode.CompareTo("Constraint") == 0) && (ecode.CompareTo("19") == 0))
                    {
                        MessageBox.Show($"Attempt to add duplicate dna.");
                        trans.Rollback();
                        if (connection.State == System.Data.ConnectionState.Closed)
                            connection.Open();
                    }
                }
                catch (Exception e)
                {
                    namesAdded.RemoveAt(i);
                    MessageBox.Show(e.Message);
                    trans.Rollback();
                    connection.Open();
                }
            }
            return rows;
        }


    }
}