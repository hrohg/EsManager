using System;
using System.Data;
using System.Data.Common;
using ES.DataAccess.Helpers;
using ES.DataAccess.Models;
using SqlConnectionFactory = ES.Business.Helpers.SqlConnectionFactory;

namespace ES.Business.Managers
{
    public class ManagerBase : Disposable
    {
        #region Properties and variables
        protected ConnectionContext ContextServer { get; private set; }
        protected ConnectionContext Context { get; private set; }
        private readonly bool _isExternalConnection;
        protected DbProviderFactory ProviderFactoryServer { get; set; }
        protected DbProviderFactory ProviderFactory { get; set; }
        #endregion

        #region Construction
        public ManagerBase()
        {
            ContextServer = new ConnectionContext(new SqlConnectionFactory().GetServerConnection());
            ProviderFactoryServer = new SqlConnectionFactory().GetServerFactory();
            Context = new ConnectionContext(new SqlConnectionFactory().GetConnection());
            ProviderFactory = new SqlConnectionFactory().GetFactory();
        }

        public ManagerBase(ConnectionContext context)
        {
            Context = context;
            _isExternalConnection = true;
            ProviderFactory = new SqlConnectionFactory().GetFactory();
        }
        #endregion
        #region Methods
        protected static EsStockDbEntities GetServerDataContext()
        {
            return new EsStockDbEntities(ApplicationManager.ServerConnectionString);
        }
        protected static EsStockDbEntities GetDataContext()
        {
            return new EsStockDbEntities(ApplicationManager.ConnectionString ?? ApplicationManager.ServerConnectionString);
        }
        protected static EsStockDbEntities GetDataContext(string connectionString)
        {
            return new EsStockDbEntities(connectionString ?? ApplicationManager.DefaultConnectionString);
        }

        protected void Execute(Action exec, bool transaction)
        {
            ValidateConnection();
            bool closeConnection = false;
            bool canManageTransaction = false;
            try
            {
                if (Context.Connection.State != ConnectionState.Open)
                {
                    Context.Connection.Open();
                    closeConnection = true;
                }
                canManageTransaction = !Context.IsTransactionStarted;
                if (canManageTransaction && transaction)
                    Context.BeginTransaction();
                exec();
                if (canManageTransaction)
                    Context.CommitTransaction();
            }
            catch (Exception)
            {
                if (Context.IsTransactionStarted && canManageTransaction)
                    Context.RollBackTransaction();
                throw;
            }
            finally
            {
                if (closeConnection)
                    Context.Connection.Close();
            }
        }
        protected T Execute<T>(Func<T> exec, bool transaction)
        {
            ValidateConnection();
            bool closeConnection = false;
            bool canManageTransaction = false;
            T retval = default(T);
            try
            {
                if (Context.Connection.State != ConnectionState.Open)
                {
                    Context.Connection.Open();
                    closeConnection = true;
                }
                canManageTransaction = !Context.IsTransactionStarted;
                if (canManageTransaction && transaction)
                    Context.BeginTransaction();
                retval = exec();
                if (canManageTransaction)
                    Context.CommitTransaction();
            }
            catch (Exception)
            {
                if (Context.IsTransactionStarted && canManageTransaction)
                    Context.RollBackTransaction();
                throw;
            }
            finally
            {
                if (closeConnection)
                    Context.Connection.Close();
            }
            return retval;
        }

        private void ValidateConnection()
        {
            if (Context == null || Context.Connection == null)
                throw new Exception("Connection does not exists");
        }
        #endregion

        #region Disposable implementation
        protected override void DisposeCore()
        {
            if (!_isExternalConnection && Context != null)
                Context.Dispose();
        }
        #endregion
    }
}

