using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Gosu.Commons.DataStructures;

namespace Gosu.Commons.Console
{
    public class ArgumentList
    {
        private readonly IEnumerable<ArgumentFlag> _flags;

        public ArgumentList(params string[] args)
        {
            var values = args.TakeWhile(x => !IsFlag(x));

            Values = new ReadOnlyCollection<string>(values.ToList());

            _flags = args.Where(IsFlag).Select(x => new ArgumentFlag(x));
        }

        private bool IsFlag(string x)
        {
            return x.StartsWith("-");
        }

        public bool HasValues
        {
            get { return Values.Count > 0; }
        }

        public ReadOnlyCollection<string> Values { get; private set; }

        public Maybe<ArgumentFlag> GetFlag(string name)
        {
            var flag = _flags.FirstOrDefault(x => x.Name == name);

            if (flag == null)
            {
                return Maybe<ArgumentFlag>.Nothing;
            }

            return flag.ToMaybe();
        }
    }
}