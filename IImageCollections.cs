using System.Collections.Generic;
using System.Threading.Tasks;

namespace WpfHashlipsJSONConverter
{
    public interface IImageCollections
    {
       int ID { get => ID; set => ID = value; }
       string CollectionName { get => CollectionName; set => CollectionName = value; }
       string Description { get => Description; set => Description = value; }
       string Name { get => Name; set => Name = value; }
       string Dcode { get => Dcode; set => Dcode = value; }
       int Minted { get => Minted; set => Minted = value; }
       int Max_Copies { get => Max_Copies; set => Max_Copies = value; }
       int Sold { get => Sold; set => Sold = value; }
       int Price { get => Price; set => Price = value; }
       string Twitter { get => Twitter; set => Twitter = "www.twitter.com/hotheadnft"; }
       string Web { get => Web; set => Web = "www.hotheadsnft.com"; }

        Task<object> CollectionBuildRecord(string nftJSONFile);

        string PrepJSONforDB(string fieldToClean);

        Task<object> AddRowFromListAsync(List<string> nftsToAdd, string SelectedCollection, string pathToDB, List<string> namesAdded);
    }
}