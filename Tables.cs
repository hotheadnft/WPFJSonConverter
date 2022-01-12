using System.Collections;
using System.Collections.Generic;

namespace WpfHashlipsJSONConverter
{
    public class Tables : IEnumerable<Tables.TableName>
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public Tables()
        {
            _name = "";
        }

        public Tables(string name)
        {
            _name = name;
        }

        public IEnumerator<TableName> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public class TableName
        {
            private string _name;

            public TableName()
            { }

            public string name { get => _name; set => _name = value; }
        }
    }
}