using Es.Data.Models;
using ES.Business.Managers;
using ES.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ES.Manager.ViewModels.Managers
{
    public class EsPartnersViewModel : DocumentViewModel
    {
        #region Internam properties
        private string _memberFilter;
        private Timer _memberTimer;
        private List<EsMemberModel> _esMembers;
        #endregion Internal properties

        #region External properties
        public List<EsMemberModel> EsMembers
        {
            get
            {
                return _esMembers != null ?
                    _esMembers.Where(s => string.IsNullOrEmpty(MemberFilter) || s.Name.ToLower().Contains(MemberFilter.ToLower())).OrderBy(s => s.Id).ToList() : new List<EsMemberModel>();
            }
            private set
            {
                _esMembers = value.ToList();
                RaisePropertyChanged("EsMembers");
            }
        }
        public ObservableCollection<EsPartnerModel> Partners { get; private set; }
        public EsPartnerModel SelectedPartner { get; set; }
        public EsMemberModel SelectedMember { get; set; }

        public int LastNumber { get; set; }
        public int Count { get; set; }

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
        #endregion External properties

        #region Constructors
        public EsPartnersViewModel()
        {
            EsMembers = MembersManager.GetEsMembers();
            Partners = new ObservableCollection<EsPartnerModel>();
            GenerateBonusCardNumberCommand = new RelayCommand(OnGenerateBonusCardNumber);
            ExportBonusCardsCommand = new RelayCommand(OnExportBonusCards, CanExportBonusCards);
        }

        #endregion constructors

        #region Internal methods
        private void DisposeMemberTimer()
        {
            if (_memberTimer != null)
            {
                _memberTimer.Dispose();
                _memberTimer = null;
            }
        }
        private void MemberTimerElapsed(object obj)
        {
            RaisePropertyChanged("EsMembers");
            DisposeMemberTimer();
        }
        private static int GenerageCheckSum(int cardindex)
        {
            int sum = 0, d;
            int a = 0;

            while (cardindex > 0)
            {
                d = cardindex % 10;
                if (a % 2 == 0)
                    d = d * 2;
                if (d > 9)
                    d -= 9;
                sum += d;
                a++;
                cardindex = cardindex / 10;
            }

            return 9 - (sum % 10);
        }
        private void OnGenerateBonusCardNumber(object obj)
        {
            Partners.Clear();
            var memberId = 1000 + (SelectedMember != null ? SelectedMember.Id : 0);
            for (int index = LastNumber + 10001; index < 10001 + LastNumber + Count; ++index)
            {
                var number = string.Format("{0}{1}{2}{3}", "0", "12014", memberId*100000 + index, GenerageCheckSum(memberId * 100000 + index));
                var numberFormating = string.Format("{0} {1} {2} {3}", number.Substring(0, 4), number.Substring(4, 4), number.Substring(8, 4), number.Substring(12, 4));
                Partners.Add(new EsPartnerModel(null) { CardNumber = number, CardNumberFormating = numberFormating });
            }
            RaisePropertyChanged("Partners");
        }
        private bool CanExportBonusCards(object obj)
        {
            return Partners.Any();
        }
        private void OnExportBonusCards(object obj)
        {
            Helpers.ExportManager.ExportProducts(Partners.ToList());
        }
        #endregion Internal methods

        #region External methods
        #endregion External methods

        #region Commands
        public ICommand GenerateBonusCardNumberCommand { get; private set; }
        public ICommand ExportBonusCardsCommand { get; private set; }
        #endregion Commands
    }
}
