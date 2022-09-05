using System;
using System.Collections.Generic;
using System.Linq;
using Es.Data.Models;
using ES.DataAccess.Models;

namespace ES.Business.Managers
{
    public class MembersManager:ManagerBase
    {
        #region Private methods

        #region Converters
        public static EsMemberModel Convert(EsMembers item)
        {
            return new EsMemberModel()
            {
                Id = item.Id,
                Name = item.Name,
                ContractNumber = item.ContractNumber
            };
        }
        public static EsMembers Convert(EsMemberModel item)
        {
            return new EsMembers
            {
                Id = item.Id,
                Name = item.Name,
                ContractNumber = item.ContractNumber
            };
        }
        public static MembersRoles Convert(MemberRoleModel item)
        {
            return new MembersRoles
            {
                Id = item.Id,
                RoleName = item.RoleName,
                Description = item.Description
            };
        }
        public static MemberRoleModel Convert(MembersRoles item)
        {
            return new MemberRoleModel
            {
                Id = item.Id,
                RoleName = item.RoleName,
                Description = item.Description
            };
        }
        #endregion Converters

        private static List<EsMembers> TryGetMembers()
        {
             using (var db = GetDataContext())
            {
                try
                {
                    return db.EsMembers.ToList();
                }
                catch (Exception)
                {
                    return new List<EsMembers>();
                }
            }
        }
        private static bool TryEdit(EsMembers item)
        {
            using (var db = GetDataContext())
            {
                try
                {
                    var exItem = db.EsMembers.SingleOrDefault(s => s.Id == item.Id ||
                        s.Name.ToLower() == item.Name.ToLower());
                    
                    if (exItem != null && item.Id != 0)
                    {
                        exItem.Id = item.Id;
                        exItem.Name = item.Name;
                        exItem.ContractNumber = item.ContractNumber;
                    }
                    else
                    {
                        if (item.Id != 0) return false;
                        item.Id = db.EsMembers.Max(s => s.Id) + 1;
                        db.EsMembers.Add(item);
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private static List<MembersRoles> TryGetMemberRoles()
        {
            using (var db = GetDataContext())
            {
                try
                {
                    return db.MembersRoles.ToList();
                }
                catch (Exception)
                {
                    return new List<MembersRoles>();
                }
            }
        }
        #endregion Private methods

        #region Public methods
        public static List<EsMemberModel> GetEsMembers()
        {
            return TryGetMembers().Select(Convert).ToList();
        }

        public static List<MemberRoleModel> GetMemberUsersRoles()
        {
            return TryGetMemberRoles().Select(Convert).ToList();
        }

        #endregion Public methods

        public static bool Edit(EsMemberModel selectedMember)
        {
            return TryEdit(Convert(selectedMember));
        }
    }
}
