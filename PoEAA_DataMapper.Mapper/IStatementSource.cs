namespace PoEAA_DataMapper.Mapper
{
    public interface IStatementSource
    {
        string Sql { get; }
        object[] Parameters { get; }
    }
}