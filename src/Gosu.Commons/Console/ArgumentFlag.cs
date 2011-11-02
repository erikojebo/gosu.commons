namespace Gosu.Commons.Console
{
    public class ArgumentFlag
    {
        public ArgumentFlag(string name)
        {
            Name = name.TrimStart('-');
        }

        public string Name { get; set; }
    }
}