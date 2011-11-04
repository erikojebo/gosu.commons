using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gosu.Commons.Console
{
    public class ArgumentFlag
    {
        public ArgumentFlag(string name, IEnumerable<string> values)
        {
            Values = new ReadOnlyCollection<string>(values.ToList());
            Name = name.TrimStart('-');
        }

        public string Name { get; set; }

        public bool HasValues
        {
            get { return Values.Count > 0; }
        }

        public ReadOnlyCollection<string > Values { get; private set; }
    }
}