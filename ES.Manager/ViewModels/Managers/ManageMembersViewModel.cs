using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Es.Data.Models;
using ES.Business.Managers;
using ES.Common.ViewModels;

namespace ES.Manager.ViewModels.Managers
{
    public class ManageMembersViewModel : DocumentViewModel
    {
        #region Internal properties
        #endregion //Internal properties

        #region External properties

        public bool IsEnabledEditMode { get { return SelectedMember != null; } }

        #region Members

        private List<EsMemberModel> _esMembers;
        public List<EsMemberModel> EsMembers
        {
            get
            {
                return _esMembers != null ?
                    _esMembers.Where(s => string.IsNullOrEmpty(MemberFilter) || s.FullName.ToLower().Contains(MemberFilter.ToLower())).OrderBy(s => s.Id).ToList() : new List<EsMemberModel>();
            }
            set
            {
                _esMembers = value.ToList();
                RaisePropertyChanged("EsMembers");
            }
        }

        private List<MemberRoleModel> _membersRoles;

        public List<MemberRoleModel> MembersRoles
        {
            get
            {
                foreach (var memberRoleModel in _membersRoles)
                {
                    memberRoleModel.IsSelected = SelectedMember != null && SelectedMemberUser != null &&
                        (_memberUsersRoles.Any(s =>
                                s.MemberId == SelectedMember.Id && s.EsUserId == SelectedMemberUser.UserId &&
                                s.MemberRoleId == memberRoleModel.Id));
                }
                return _membersRoles;
            }
        }

        #region Filters

        private Timer _memberTimer;

        private void MemberTimerElapsed(object obj)
        {
            RaisePropertyChanged("EsMembers");
            DisposeMemberTimer();
        }
        private void DisposeMemberTimer()
        {
            if (_memberTimer != null)
            {
                _memberTimer.Dispose();
                _memberTimer = null;
            }
        }

        private string _memberFilter;
        public string MemberFilter
        {
            get { return _memberFilter ?? string.Empty; }
            set
            {
                _memberFilter = value.ToLower();
                RaisePropertyChanged("MemberFilter");
                DisposeMemberTimer();
                _memberTimer = new Timer(MemberTimerElapsed, null, 300, 300);
            }
        }

        #endregion Filters

        #region Selected member
        private EsMemberModel _selectedMember;
        public EsMemberModel SelectedMember
        {
            get { return _selectedMember; }
            set
            {
                _selectedMember = value;

                RaisePropertyChanged("SelectedMember");
                RaisePropertyChanged("IsEnabledEditMode");
                RaisePropertyChanged("EditButonContent");

                if (_selectedMember == null)
                {
                    _memberUsersRoles = new List<MemberUsersRolesModel>();
                }
                else
                {
                    _memberUsersRoles = UsersManager.GetMemberUsersRoles(_selectedMember.Id);

                }
                _memberUsers = _memberUsersRoles != null ? _memberUsersRoles.OrderBy(s => s.EsUsers.FullName).GroupBy(s => s.EsUserId).Select(s => s.First().EsUsers).ToList() : new List<EsUserModel>();

                RaisePropertyChanged("SelectedMemberUser");
                RaisePropertyChanged("MemberUsers");
                RaisePropertyChanged("MembersRoles");
            }
        }

        public string EditButonContent { get { return (SelectedMember == null || SelectedMember.Id == 0) ? "Ավելացնել" : "Խմբագրել"; } }

        #endregion //Selected member

        #endregion Members

        #region Member Users
        private List<MemberUsersRolesModel> _memberUsersRoles;
        private List<EsUserModel> _memberUsers;

        public List<EsUserModel> MemberUsers
        {
            get
            {
                return _memberUsers.Where(s => string.IsNullOrEmpty(MemberUserFilter) || s.FullName.ToLower().Contains(MemberUserFilter.ToLower()) || s.Email.ToLower().Contains(MemberUserFilter.ToLower())).ToList();
            }
            set
            {
                _memberUsers = value;
                SelectedMemberUser = null;
                RaisePropertyChanged("MemberUsers");
            }
        }

        private EsUserModel _selectedMemberUser;
        public EsUserModel SelectedMemberUser
        {
            get { return _selectedMemberUser; }
            set
            {
                _selectedMemberUser = value;
                RaisePropertyChanged("SelectedMemberUser");
                RaisePropertyChanged("MembersRoles");
            }
        }

        #region Member User Filters

        private Timer _memberUserTimer;

        private void MemberUserTimerElapsed(object obj)
        {
            RaisePropertyChanged("MemberUsers");
            DisposeMemberUserTimer();
        }
        private void DisposeMemberUserTimer()
        {
            if (_memberUserTimer != null)
            {
                _memberUserTimer.Dispose();
                _memberUserTimer = null;
            }
        }

        private string _memberUserFilter;
        public string MemberUserFilter
        {
            get { return _memberUserFilter ?? string.Empty; }
            set
            {
                _memberUserFilter = value.ToLower();
                RaisePropertyChanged("MemberUserFilter");
                DisposeMemberUserTimer();
                _memberUserTimer = new Timer(MemberUserTimerElapsed, null, 300, 300);
            }
        }

        #endregion Member User Filters

        #endregion Member Users

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

        #region Selected user
        private EsUserModel _selectedEsUser;
        public EsUserModel SelectedEsUser
        {
            get { return _selectedEsUser; }
            set
            {
                _selectedEsUser = value;
                RaisePropertyChanged("SelectedEsUser");
            }
        }

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

        #endregion Users

        #endregion //External properties

        #region Constants

        #endregion //Constants

        #region Constructors
        public ManageMembersViewModel()
        {
            Initialize();
        }
        #endregion //Constructors

        #region Internal methods

        private void Initialize()
        {
            Title = "Կազամկերպությունների խմբագրում";
            Load();
        }

        private void Load()
        {
            _membersRoles = MembersManager.GetMemberUsersRoles();

            SelectedMember = null;
            EsMembers = MembersManager.GetEsMembers();

            SelectedEsUser = null;
            EsUsers = UsersManager.GetEsUsers();
        }


        #endregion //Internal methods

        #region External methods
        #endregion //External methods

        #region Commands
        //New member
        private ICommand _newCommand;
        public ICommand NewCommand { get { return _newCommand ?? (_newCommand = new RelayCommand(OnNew, CanNew)); } }
        private bool CanNew(object o)
        {
            return SelectedMember == null || !string.IsNullOrEmpty(SelectedMember.FullName);
        }
        private void OnNew(object o)
        {
            if (CanNew(o))
            {
                SelectedMember = new EsMemberModel();
            }
        }

        //Edit Member
        private ICommand _editCommand;
        public ICommand EditCommand { get { return _editCommand ?? (_editCommand = new RelayCommand(OnEdit, CanEdit)); } }
        private bool CanEdit(object o)
        {
            return SelectedMember != null && !string.IsNullOrEmpty(SelectedMember.FullName);
        }
        private void OnEdit(object o)
        {
            if (CanEdit(o))
            {
                if (MembersManager.Edit(SelectedMember))
                {
                    SelectedMember = null;
                    EsMembers = MembersManager.GetEsMembers();
                }
                else
                {
                    MessageBox.Show("Գրանցման սխալ", "Գործողության ձախողում", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        //Add user
        private ICommand _addUserCommand;
        public ICommand AddUserCommand { get { return _addUserCommand ?? (_addUserCommand = new RelayCommand(OnAddUser, CanAddUser)); } }
        private bool CanAddUser(object o)
        {
            return SelectedMember != null && SelectedMember.Id != 0 && SelectedEsUser != null && _memberUsers.All(s => s.UserId != SelectedEsUser.UserId);
        }
        private void OnAddUser(object o)
        {
            if (CanAddUser(o))
            {
                _memberUsers.Add(SelectedEsUser);
                RaisePropertyChanged("MemberUsers");
            }
        }

        //Edit user role
        private ICommand _editUserCommand;
        public ICommand EditUserCommand { get { return _editUserCommand ?? (_editUserCommand = new RelayCommand(OnEditUser, CanEditUser)); } }

        private bool CanEditUser(object obj)
        {
            return SelectedMember != null && SelectedMemberUser != null;
        }

        private void OnEditUser(object obj)
        {
            if (UsersManager.EditUserRoles(SelectedMemberUser.UserId,
                _membersRoles.Where(s => s.IsSelected).Select(s => s.Id), SelectedMember.Id))
            {
                var member = SelectedMember;
                SelectedMember = null;
                SelectedMember = member;
            }
            else
            {
                MessageBox.Show("Գրանցման սխալ", "Գործողության ձախողում", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //Remove user role
        private ICommand _removeUserCommand;
        public ICommand RemoveUserCommand { get { return _removeUserCommand ?? (_removeUserCommand = new RelayCommand(OnRemoveUser, CanRemoveUser)); } }

        private bool CanRemoveUser(object o)
        {
            return SelectedMemberUser!=null;
        }

        private void OnRemoveUser(object o)
        {
            if (CanRemoveUser(o))
            {
                _memberUsers.Remove(SelectedMemberUser);
                RaisePropertyChanged("MemberUsers");
            }
        }
        #endregion //Commands
    }
}
