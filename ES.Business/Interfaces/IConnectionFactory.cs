using System.Data.Common;

namespace ES.Business.Interfaces
{
    public interface IConnectionFactory
    {
        DbConnection GetConnection();
        DbProviderFactory GetFactory();
    }
}
