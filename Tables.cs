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
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public class TableName:IEnumerator
        {
            private string _name;
            public TableName(string name)
            {
                _name=name;
            }
            public TableName()
            { }

            public string name { get => _name; set => _name = value; }
            public object Current { get; }
            public List<TableName> tableNames { get; set; }
            public TableName(TableName[] tarray)
            {
                tableNames = new List<TableName>();
                for (int i = 0; i < tarray.Length; i++)
                {
                    tableNames[i] = tarray[i];
                }
            }

            public bool MoveNext()
            {
                throw new System.NotImplementedException();
            }

            public void Reset()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}