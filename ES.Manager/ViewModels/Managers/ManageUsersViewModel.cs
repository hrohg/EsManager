using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Es.Data.Models;
using ES.Business.Managers;
using ES.Common.ViewModels;

namespace ES.Manager.ViewModels.Managers
{
    public class ManageUsersViewModel : DocumentViewModel
    {
        #region Internal properties
        #endregion //Internal properties

        #region External properties

        #region User mail or phone
        private string _userMailOrPhone;
        public string UserEmailOrPhone { get { return _userMailOrPhone; } set { _userMailOrPhone = value; RaisePropertyChanged("UserEmailOrPhone"); } }
        #endregion User mail or phone

        public bool IsEnabledEditMode { get { return SelectedEsUser != null; } }

        #region Users
        private List<EsUserModel> _esUsers;
        public List<EsUserModel> EsUsers
        {
            get
            {
                return _esUsers != null ?
                    _esUsers.Where(s => string.IsNullOrEmpty(UsersFilter) || s.FullName.ToLower().Contains(UsersFilter.ToLower()) || s.Email.ToLower().Contains(UsersFilter.ToLower())).OrderBy(s => s.FullName).ToList() : new List<EsUserModel>();
            }
            set
            {
                _esUsers = value.ToList();
                RaisePropertyChanged("EsUsers");
            }
        }
        #endregion Users

        #region Selected user
        private EsUserModel _selectedEsUser;
        public EsUserModel SelectedEsUser
        {
            get { return _selectedEsUser; }
            set
            {
                if (_selectedEsUser != null)
                {
                    _selectedEsUser.IsClearPassword = true;
                    _selectedEsUser.NewPassword = string.Empty;
                    _selectedEsUser.IsClearPassword = false;
                }

                _selectedEsUser = value;

                RaisePropertyChanged("SelectedEsUser");
                RaisePropertyChanged("IsEnabledEditMode");
                RaisePropertyChanged("EditButonContent");
            }
        }

        public string EditButonContent { get { return (SelectedEsUser == null || SelectedEsUser.UserId == 0) ? "Ավելացնել" : "Խմբագրել"; } }
        #endregion Selected user

        #region Filters

        private Timer _usersTimer;

        private void UsersTimerElapsed(object obj)
        {
            RaisePropertyChanged("EsUsers");
            DisposeUsersTimer();
        }
        private void DisposeUsersTimer()
        {
            if (_usersTimer != null)
            {
                _usersTimer.Dispose();
                _usersTimer = null;
            }
        }

        private string _usersFilter;
        public string UsersFilter
        {
            get { return _usersFilter ?? string.Empty; }
            set
            {
                _usersFilter = value.ToLower();
                RaisePropertyChanged("UsersFilter");
                DisposeUsersTimer();
                _usersTimer = new Timer(UsersTimerElapsed, null, 300, 300);
            }
        }

        #endregion Filters

        #endregion //External properties

        #region Constants

        #endregion //Constants

        #region Constructors
        public ManageUsersViewModel()
        {
            Initialize();
        }
        #endregion //Constructors

        #region Internal methods

        private void Initialize()
        {
            Title = "Օգտագործողների խմբագրում";
            Load();
        }

        private void Load()
        {
            SelectedEsUser = null;
            EsUsers = UsersManager.GetEsUsers();
        }


        #endregion //Internal methods

        #region External methods
        #endregion //External methods

        #region Commands
        private ICommand _newUserCommand;
        public ICommand NewUserCommand { get { return _newUserCommand ?? (_newUserCommand = new RelayCommand(OnNewtUser, CanNewUser)); } }
        private bool CanNewUser(object o)
        {
            return SelectedEsUser == null || !string.IsNullOrEmpty(SelectedEsUser.FullName);
        }
        private void OnNewtUser(object o)
        {
            if (CanNewUser(o))
            {
                SelectedEsUser = new EsUserModel();
            }
        }

        private ICommand _editUserCommand;
        public ICommand EditUserCommand { get { return _editUserCommand ?? (_editUserCommand = new RelayCommand(OnEditUser, CanEditUser)); } }
        private bool CanEditUser(object o)
        {
            return SelectedEsUser != null && !string.IsNullOrEmpty(SelectedEsUser.UserName) &&
                !string.IsNullOrEmpty(SelectedEsUser.Email);
        }
        private void OnEditUser(object o)
        {
            if (CanEditUser(o))
            {
                if (UsersManager.EditUser(SelectedEsUser))
                {
                    SelectedEsUser = null;
                    EsUsers = UsersManager.GetEsUsers();
                }
                else
                {
                    MessageBox.Show("Գրանցման սխալ", "Գործողության ձախողում", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        #endregion //Commands

    }
}
