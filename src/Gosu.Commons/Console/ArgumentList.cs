using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Gosu.Commons.DataStructures;

namespace Gosu.Commons.Console
{
    public class ArgumentList
    {
        private readonly IList<ArgumentFlag> _flags = new List<ArgumentFlag>();
        private readonly string[] _args;

        public ArgumentList(params string[] args)
        {
            _args = args;

            IEnumerable<string> values = GetValuesStartingAt(0);

            Values = new ReadOnlyCollection<string>(values.ToList());

            for (int i = 0; i < args.Length; i++)
            {
                var value = args[i];

                if (IsFlag(value))
                {
                    var flagValues = GetValuesStartingAt(i + 1);
                    _flags.Add(new ArgumentFlag(value, flagValues));
                }
            }
        }

        private IEnumerable<string> GetValuesStartingAt(int index)
        {
            return _args.Skip(index).TakeWhile(x => !IsFlag(x));
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

        public Maybe<string> GetFlagValue(string flagName)
        {
            var flag = GetFlag(flagName);

            if (flag.IsNothing || !flag.Value.HasValues)
            {
                return Maybe<string>.Nothing;
            }

            return flag.Value.Values[0].ToMaybe();
        }

        public string GetFlagValueOrDefault(string flagName, string defaultValue)
        {
            var flagValue = GetFlagValue(flagName);

            if (flagValue.HasValue)
            {
                return flagValue.Value;
            }

            return defaultValue;
        }
    }
}