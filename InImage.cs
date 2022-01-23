using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace WpfHashlipsJSONConverter
{
    internal class InImage
    {
        private int _id;
        private string _background;
        private string _colorDepth;
        private string _dimensions;
        private string _dcode;
        private string _twitter;
        private string _web;
        private string _name;
        private string _description;
        private int _price;
        private int _sold;
        private int _max_Copies;
        private int _minted;
        private string _collectionName;
        public int ID { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string Background { get => _background; set => _background = value; }
        public string ColorDepth { get => _colorDepth; set => _colorDepth = value; }
        public string Dimensions { get => _dimensions; set => _dimensions = value; }
        public string Dcode { get => _dcode; set => _dcode = value; }
        public string Twitter { get => _twitter; set => _twitter = value; }
        public string Web { get => _web; set => _web = value; }
        public string CollectionName { get => _collectionName; set => _collectionName = value; }
        public int Minted { get => _minted; set => _minted = value; }
        public int Max_Copies { get => _max_Copies; set => _max_Copies = value; }
        public int Sold { get => _sold; set => _sold = value; }
        public int Price { get => _price; set => _price = value; }
        public string Description { get => _description; set => _description = value; }

        public InImage()
        {
            _id = 0;
            _name = string.Empty;
            _description = "Hotheads!";
            _max_Copies = 0;
            _sold = 0;
            _twitter = string.Empty;
            _minted = 0;
            _web = string.Empty;
            _collectionName = "InImage";
            _background = string.Empty;
            _colorDepth = string.Empty;
            _dimensions = string.Empty;
            _dcode = string.Empty;
            _twitter = "https://twitter.com/hotheadnft.com";
            _web = "Https://wwww.hotheadsnft.com";
        }

        public static InImage CollectionBuildRecord(string nftJSONFile)
        {
            try
            {
                InImage currentCollection = new();
                var NftMakerToConvert = File.ReadAllLines(nftJSONFile);

                currentCollection.ID = 0;
                currentCollection.Name = PrepJSONforDB(NftMakerToConvert[4]);
                currentCollection.Description = PrepJSONforDB(NftMakerToConvert[7]);
                currentCollection.Background = PrepJSONforDB(NftMakerToConvert[15]);
                currentCollection.ColorDepth = PrepJSONforDB(NftMakerToConvert[16]);
                currentCollection.Dimensions = PrepJSONforDB(NftMakerToConvert[17]);
                currentCollection.Dcode = PrepJSONforDB(NftMakerToConvert[18]);
                currentCollection.Twitter = PrepJSONforDB(NftMakerToConvert[19]);
                currentCollection.Web = PrepJSONforDB(NftMakerToConvert[20]);

                currentCollection.Price = 100;
                currentCollection.Sold = 0;
                currentCollection.Max_Copies = 50;
                currentCollection.Minted = 0;

                return currentCollection;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }

        private static string PrepJSONforDB(string fieldToClean)
        {
            string jsonBuffer;
            string[] json_parts;
            json_parts = fieldToClean.Split(':');
            jsonBuffer = json_parts[1];
            jsonBuffer = jsonBuffer.Replace(",", String.Empty);
            jsonBuffer = jsonBuffer.Replace("\t", String.Empty);
            jsonBuffer = jsonBuffer.Replace("\"", String.Empty);
            return jsonBuffer;
        }

        public static int AddRow(InImage nftToAdd, string selectedcollection, string pathToDB)
        {
            string background, collectionname, colorDepth, dimensions, dcode, description, twitter, web, name;
            int total_minted = 0, rows = 0;
            int price, max_copies = 0, sold;
            colorDepth = nftToAdd.ColorDepth;
            background = nftToAdd.Background;
            dimensions = nftToAdd.Dimensions;
            collectionname = selectedcollection;
            name = nftToAdd.Name;
            dcode = nftToAdd.Dcode;
            twitter = nftToAdd.Twitter;
            web = nftToAdd.Web;

            price = nftToAdd.Price;
            description = nftToAdd.Description;
            sold = 0;
            string currdir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(pathToDB);
            string dbfile = "URI=file:NFTDB.db";
            SQLiteConnection connection = new(dbfile);
            connection.Open();
            Directory.SetCurrentDirectory(currdir);

            string addCollection = $"insert into {selectedcollection}(id,name,description,colorDepth,dimensions,background,total_minted,price,sold,max_copies,dcode,twitter,web,collectionname)" +
                "VALUES (@id,@name,@description,@colorDepth,@dimensions,@background,@total_minted,@price,@sold,@max_copies,@dcode,@twitter,@web,@collectionname);";
            var trans = connection.BeginTransaction();
            SQLiteCommand command = new(addCollection, connection);

            command.Parameters.AddWithValue("@id", null);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@colorDepth", colorDepth);
            command.Parameters.AddWithValue("@dimensions", dimensions);
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
                    return rows;
                }
                else
                {
                    MessageBox.Show(sqc.Message);
                    trans.Rollback();
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
                    return rows;
                }
                else
                {
                    MessageBox.Show(sqc.Message);
                    trans.Rollback();
                    return rows;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                trans.Rollback();
                return rows;
            }
            MessageBox.Show("Successfully added one record");
            connection.Close();
            return rows;
        }

        public static int AddRowFromList(List<string> nftsToAdd, string selectedcollection, string pathToDB, List<string> namesAdded)
        {
            string background, collectionname, colorDepth, dimensions, dcode, description, twitter, web, name;
            int total_minted = 0;
            int price, max_copies = 0, sold;
            int rows = 0;
            // List<string> namesAdded = new List<string>();
            //  string currdir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(pathToDB);
            string dbfile = "URI=file:NFTDB.db";
            SQLiteConnection connection = new(dbfile);
            connection.Open();
            string addCollection = $"insert into {selectedcollection}(id,name,description,colorDepth,dimensions,background,total_minted,price,sold,max_copies,dcode,twitter,web,collectionname)" +
                 "VALUES (@id,@name,@description,@colorDepth,@dimensions,@background,@total_minted,@price,@sold,@max_copies,@dcode,@twitter,@web,@collectionname);";

            SQLiteCommand command;

            for (int i = 0; i < nftsToAdd.Count; i++)
            {
                var trans = connection.BeginTransaction();

                command = new SQLiteCommand(addCollection, connection);

                InImage inImageToAdd = InImage.CollectionBuildRecord(nftsToAdd[i]);
                colorDepth = inImageToAdd.ColorDepth;
                background = inImageToAdd.Background;
                dimensions = inImageToAdd.Dimensions;
                name = inImageToAdd.Name;
                dcode = inImageToAdd.Dcode;
                twitter = inImageToAdd.Twitter;
                web = inImageToAdd.Web;

                price = inImageToAdd.Price;
                description = inImageToAdd.Description;

                collectionname = selectedcollection;
                sold = 0;
                command.Parameters.AddWithValue("@id", null);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@colorDepth", colorDepth);
                command.Parameters.AddWithValue("@dimensions", dimensions);
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
            return rows;
        }
    }
}