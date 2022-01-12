using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHashlipsJSONConverter
{
    internal class Eyeball9
    {
        private int _id;
        private string _description;
        private string _background;
        private string _eyeball;
        private string _eyecolor;
        private string _iris;
        private string _shine;
        private string _bottom_lid;
        private string _top_lid;
        private string _dcode;
        private string _twitter;
        private string _web;
        private string _name;

        private string _collectionName;
        private int _total_minted;
        private int _max_Copies;
        private int _sold;
        private int _price;
        private static int _recordCount;

        public static int recordCount { get => _recordCount; set => _recordCount = value; }

        public int ID { get => _id; set => _id = value; }
        public string Background { get => _background; set => _background = value; }
        public string Eyeball { get => _eyeball; set => _eyeball = value; }
        public string Eyecolor { get => _eyecolor; set => _eyecolor = value; }
        public string Iris { get => _iris; set => _iris = value; }
        public string Shine { get => _shine; set => _shine = value; }
        public string Bottom_lid { get => _bottom_lid; set => _bottom_lid = value; }
        public string Top_lid { get => _top_lid; set => _top_lid = value; }
        public string Dcode { get => _dcode; set => _dcode = value; }
        public string Twitter { get => _twitter; set => _twitter = value; }
        public string Web { get => _web; set => _web = value; }
        public string Name { get => _name; set => _name = value; }
        public string CollectionName { get => _collectionName; set => _collectionName = value; }
        public int Minted { get => _total_minted; set => _total_minted = value; }
        public int Max_Copies { get => _max_Copies; set => _max_Copies = value; }
        public int Sold { get => _sold; set => _sold = value; }
        public int Price { get => _price; set => _price = value; }
        public string Description { get => _description; set => _description = value; }

        public Eyeball9()
        {
            _background = string.Empty;
            _description = string.Empty;
            _bottom_lid = string.Empty;
            _shine = string.Empty;
            _top_lid = string.Empty;
            _collectionName = string.Empty;
            _dcode = string.Empty;
            _eyeball = string.Empty;
            _eyecolor = string.Empty;
            _iris = string.Empty;
            _id = 0;
            _name = string.Empty;
            _max_Copies = 0;
            _sold = 0;
            _twitter = string.Empty;
            _total_minted = 0;
            _web = string.Empty;
        }

        public static Eyeball9 CollectionBuildRecord(string nftJSONFile)
        {
            try
            {
                Eyeball9 currentCollection = new();
                var NftMakerToConvert = File.ReadAllLines(nftJSONFile);
                currentCollection.ID = 0;
                currentCollection.Name = PrepJSONforDB(NftMakerToConvert[4]);
                currentCollection.Description = PrepJSONforDB(NftMakerToConvert[7]);
                currentCollection.Background = PrepJSONforDB(NftMakerToConvert[15]);
                currentCollection.Eyeball = PrepJSONforDB(NftMakerToConvert[16]);
                currentCollection.Eyecolor = PrepJSONforDB(NftMakerToConvert[17]);
                currentCollection.Iris = PrepJSONforDB(NftMakerToConvert[18]);
                currentCollection.Shine = PrepJSONforDB(NftMakerToConvert[19]);
                currentCollection.Bottom_lid = PrepJSONforDB(NftMakerToConvert[20]);
                currentCollection.Top_lid = PrepJSONforDB(NftMakerToConvert[21]);
                currentCollection.Dcode = PrepJSONforDB(NftMakerToConvert[22]);
                currentCollection.Twitter = PrepJSONforDB(NftMakerToConvert[23]);
                currentCollection.Web = PrepJSONforDB(NftMakerToConvert[24]);

                currentCollection.Price = 100;
                currentCollection.Sold = 0;
                currentCollection.Max_Copies = 50;
                currentCollection.Minted = 0;
                return currentCollection;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        private static string PrepJSONforDB(string fieldToClean)
        {
            string jsonBuffer = fieldToClean;
            string[] json_parts;
            json_parts = fieldToClean.Split(':');
            if (json_parts.Length > 1)
            {
                jsonBuffer = json_parts[1];
                jsonBuffer = jsonBuffer.Replace(",", String.Empty);
                jsonBuffer = jsonBuffer.Replace("\t", String.Empty);
                jsonBuffer = jsonBuffer.Replace("\"", String.Empty);
            }
            return jsonBuffer;
        }

        public static int AddRow(Eyeball9 nftToAdd, string SelectedCollection, string pathToDB)
        {
            Stopwatch sw = new();
            sw.Start();
            long timetaken;
            string background, eyeball, eyecolor, iris, shine, bottom_lid, top_lid, dcode, collectionname, twitter, web, name;
            int price = nftToAdd.Price, rows = 0;
            string description;
            description = nftToAdd.Description;
            int sold, max_copies, total_minted;
            sold = max_copies = total_minted = 0;
            dcode = nftToAdd.Dcode;
            twitter = nftToAdd.Twitter;
            web = nftToAdd.Web;
            eyeball = nftToAdd.Eyeball;
            background = nftToAdd.Background;
            eyecolor = nftToAdd.Eyecolor;
            iris = nftToAdd.Iris;
            shine = nftToAdd.Shine;
            bottom_lid = nftToAdd.Bottom_lid;
            top_lid = nftToAdd.Top_lid;
            collectionname = SelectedCollection;
            name = nftToAdd.Name;
            string currdir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(pathToDB);
            string dbfile = "URI=file:NFTDB.db";
            using SQLiteConnection connection = new(dbfile);
            connection.Open();
            Directory.SetCurrentDirectory(currdir);
            var trans = connection.BeginTransaction();
            string addCollection = "insert into Eyeball9(id,name,description,eyeball,eyecolor,iris,shine,bottom_lid,top_lid,background,dcode,twitter,web,price,sold,max_copies,total_minted,collectionname)" +
                         "VALUES (@id,@name,@description,@eyeball,@eyecolor,@iris,@shine,@bottom_lid,@top_lid,@background,@dcode,@twitter,@web,@price,@sold,@max_copies,@total_minted,@collectionname);";

            SQLiteCommand command = new(addCollection, connection);

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
                rows = command.ExecuteNonQuery();
            }
            catch (SQLiteException sqc)
            {
                string ecode = sqc.ErrorCode.ToString();
                string rcode = sqc.ResultCode.ToString();
                if ((rcode.CompareTo("Constraint") == 0) && (ecode.CompareTo("19") == 0))
                {
                    MessageBox.Show($"Attempt to add duplicate dna.");
                    trans.Rollback();
                    sw.Stop();
                    timetaken = sw.ElapsedMilliseconds;
                    Debug.WriteLine($"{ timetaken } ");
                    return rows;
                }
                else
                {
                    MessageBox.Show(sqc.Message);
                    trans.Rollback();
                    sw.Stop();
                    timetaken = sw.ElapsedMilliseconds;
                    Debug.WriteLine($"{ timetaken } ");
                    return rows;
                }
            }
            try
            {
                trans.Commit();
            }
            catch (SQLiteException sqc)
            {
                string ecode = sqc.ErrorCode.ToString();
                string rcode = sqc.ResultCode.ToString();
                if ((rcode.CompareTo("Constraint") == 0) && (ecode.CompareTo("19") == 0))
                {
                    MessageBox.Show($"Attempt to add duplicate dna.");
                    trans.Rollback();
                    sw.Stop();
                    timetaken = sw.ElapsedMilliseconds;
                    Debug.WriteLine($"{ timetaken } ");
                    return rows;
                }
                else
                {
                    MessageBox.Show(sqc.Message);
                    trans.Rollback();
                    sw.Stop();
                    timetaken = sw.ElapsedMilliseconds;
                    Debug.WriteLine($"{ timetaken } ");
                    return rows;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                trans.Rollback();
                sw.Stop();
                timetaken = sw.ElapsedMilliseconds;
                Debug.WriteLine($"{ timetaken } ");
                return rows;
            }
            //  MessageBox.Show("Successfully added one record");
            sw.Stop();
            timetaken = sw.ElapsedMilliseconds;
            Debug.WriteLine($"{ timetaken } ");
            connection.Close();
            return rows;
        }

        public static int AddRowFromList(List<string> nftsToAdd, string SelectedCollection, string pathToDB, List<string> namesAdded)
        {
            Stopwatch sw = new();
            sw.Start();
            string background, eyeball, eyecolor, iris, shine, bottom_lid, top_lid, dcode, collectionname, twitter, web, name, description;
            int price, rows = 0;
            //      string currdir = Directory.GetCurrentDirectory();
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

                Eyeball9 eyeballToAdd = Eyeball9.CollectionBuildRecord(nftsToAdd[i]);

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
                    rows += command.ExecuteNonQuery();
                    trans.Commit();
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
            sw.Stop();
            long timetaken = sw.ElapsedMilliseconds;
            Debug.WriteLine($"{ timetaken } ");
            return rows;
        }

        public static async Task<int> AddRowFromListAsync(List<string> nftsToAdd, string SelectedCollection, string pathToDB, List<string> namesAdded)
        {
            Stopwatch sw = new();
            sw.Start();
            string background, eyeball, eyecolor, iris, shine, bottom_lid, top_lid, dcode, collectionname, twitter, web, name, description;
            int price, rows = 0;
            //      string currdir = Directory.GetCurrentDirectory();
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

                Eyeball9 eyeballToAdd = Eyeball9.CollectionBuildRecord(nftsToAdd[i]);

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

            sw.Stop();
            long timetaken = sw.ElapsedMilliseconds;
            Debug.WriteLine($"{ timetaken } ");
            return rows;
        }
    }
}