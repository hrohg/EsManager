namespace ES.Business.Models
{
    public class DataServer
    {
        #region internal properties

        private string _providerName = "System.Data.SqlClient";
        private string _connectionMetadata = @"res://*/Models.EsStockDbModel.csdl|res://*/Models.EsStockDbModel.ssdl|res://*/Models.EsStockDbModel.msl";
        private bool _integratedSecurity = false;
        private bool _persistSecurityInfo = true;
        private bool _multipleActiveResultSets = true;
        #endregion
        public string Description { get; set; }
        public string Name { get; set; }
        public string Instance { get; set; }
        public int? Port { get; set; }
        public string Database { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ProviderName { get { return _providerName; } set { if (string.Equals(value, _providerName)) { return; } _providerName = value; } }
        public string ConnectionMetadata { get { return _connectionMetadata; } set { if (string.Equals(value, _connectionMetadata)) { return; } _connectionMetadata = value; } }
        public bool IntegratedSecurity { get { return _integratedSecurity; } set { _integratedSecurity = value; } }
        public bool PersistSecurityInfo { get { return _persistSecurityInfo; } set { _persistSecurityInfo = value; } }
        public bool MultipleActiveResultSets { get { return _multipleActiveResultSets; } set { _multipleActiveResultSets = value; } }

        public DataServer()
        {

        }
        public DataServer(string description, string serverName)
        {
            Description = description;
            Name = serverName;
        }
    }
}