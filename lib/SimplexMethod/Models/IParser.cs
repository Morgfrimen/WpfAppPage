namespace SimplexMethod.Models
{

    public interface IParser
    {
        string StrParse { get; }
        IMethod GetResult();

        void ParseStr(string strparse);
    }

}
