namespace PoEAA_DataMapper
{
    internal interface IStatementSource
    {
        string Sql { get; }
        object[] Parameters { get; }
    }
}