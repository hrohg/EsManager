using System.ComponentModel;

namespace Es.Data.Models
{
    public class EsMemberModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Properties
        /// </summary>
        #region Properties
        private const string IdProperty = "Id";
        private const string NameProperty = "Name";
        private const string ContractNumberProperty = "ContractNumber";
        #endregion
        /// <summary>
        /// Private properties
        /// </summary>
        #region Private properties
        private int _id = 0;
        private string _fullName;
        private string _email;
        private string _contractNumber;
        #endregion
        /// <summary>
        /// Public properties
        /// </summary>
        #region Public properties
        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(IdProperty); } }
        public string Name { get { return _fullName; } set { _fullName = value; OnPropertyChanged(NameProperty); } }
        public string ContractNumber { get { return _contractNumber; } set { _contractNumber = value; OnPropertyChanged(ContractNumberProperty); } }
        #endregion
        /// <summary>
        /// Property changed
        /// </summary>
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
