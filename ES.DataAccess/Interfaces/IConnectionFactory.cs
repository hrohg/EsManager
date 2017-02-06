using System.Data.Common;

namespace ES.DataAccess.Interfaces
{
    public interface IConnectionFactory
    {
        DbConnection GetConnection();
        DbProviderFactory GetFactory();
    }
}
