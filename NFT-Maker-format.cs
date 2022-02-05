using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WpfHashlipsJSONConverter
{
    internal class NFT_Maker_format
    {
        private string _dna;
        private string _name;
        private string _description;
        private List<string> _trait_type;
        private List<string> _trait_value;
      
        public string dna { get => _dna; set => _dna = value; }
        public string name { get => _name; set => _name = value; }
        public string description { get => _description; set => _description = value; }
        public List<string> trait_type { get => _trait_type; set => _trait_type = value; }
        public List<string> trait_value { get => _trait_value; set => _trait_value = value; }

        public NFT_Maker_format()
        {
            trait_type = new List<string>();
            trait_value = new List<string>();
        }

        public void parseAttributes(string[] jsonContent)
        {
            trait_type.Clear();
            trait_value.Clear();
            string line, cleanedTrait, cleanedValue;
            StringBuilder attribute = new StringBuilder();
            string[] line_parts;
            //read array until attributes section
            int i = 0;
            int traits, startOfAttributes, endOfAttributes;
            startOfAttributes = endOfAttributes = traits = 1;
            //get description from line 2
            description = "\"description\": \"Hotheads!\"";
            dna = jsonContent[1];
            name = jsonContent[2];
           
            while (startOfAttributes != 0)
            {
                // Console.WriteLine(jsonContent[i]);
                startOfAttributes = jsonContent[i].IndexOf("  \"attributes\": [");
                i++;
            }
            i++;
            endOfAttributes = i;
            line = jsonContent[i];
            while (endOfAttributes != 0)
            {
                traits = line.IndexOf("      \"trait_type\"");

                if (traits >= 0)
                {
                    line_parts = line.Split(':');

                    cleanedTrait = line_parts[1].Replace(",", ":");
               
                    i++;
                    line = jsonContent[i];
                    line_parts = line.Split(':');

                    cleanedValue = line_parts[1];
                    cleanedValue = cleanedValue.Trim();
                    i++;
                    line = jsonContent[i];
                    endOfAttributes = line.IndexOf("  ],");
                    attribute.Clear();
                    attribute.Append(cleanedTrait + cleanedValue);
                    if (endOfAttributes != 0)
                    {

                        trait_type.Add(attribute + "," + Environment.NewLine);
                    }
                    else
                    {

                        trait_type.Add(attribute + Environment.NewLine);
                    }

                    continue;
                }
                i++;

                line = jsonContent[i];
                endOfAttributes = line.IndexOf("  ],");

            }
            //add dna, twitter and web addresses as traits
            attribute.Clear();
            attribute.Append(dna.Trim());
            trait_type.Add(" "+attribute + Environment.NewLine);

            trait_type.Add(" \"Twitter\":\"twitter.com/hotheadnft\"" + "," + Environment.NewLine);
            trait_type.Add(" \"Website\":\"www.hotheadsnft.com\"" + Environment.NewLine);
            //get last entry in list
            // trait_type.RemoveAt(trait_type.Count-1);
            // trait_type.Add(attribute + Environment.NewLine);
        }
    }
}