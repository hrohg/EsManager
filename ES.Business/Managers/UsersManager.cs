using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Es.Data.Models;
using ES.DataAccess.Models;

namespace ES.Business.Managers
{
    public class UsersManager : ManagerBase
    {
        #region Private methods

        #region Converters
        private static EsUserModel Convert(EsUsers item)
        {
            if (item == null) return null;
            return new EsUserModel
            {
                UserId = item.UserId,
                IsActive = item.IsActive,
                UserName = item.UserName,
                Email = item.Email,
                Mobile = item.Mobile,
                ClubSixteenId = item.ClubSixteenId,
                LastActivityDate = item.LastActivityDate
            };
        }
        private static EsUsers Convert(EsUserModel item)
        {
            if (item == null) return null;
            return new EsUsers
            {
                UserId = item.UserId,
                IsActive = item.IsActive,
                UserName = item.UserName,
                Email = item.Email,
                Mobile = item.Mobile,
                ClubSixteenId = item.ClubSixteenId,
                LastActivityDate = item.LastActivityDate,
                Password = EncodePassword(item.NewPassword)
            };
        }

        private static MemberUsersRoles Convert(MemberUsersRolesModel item)
        {
            if (item == null) return null;
            return new MemberUsersRoles
            {
                Id = item.Id,
                EsUserId = item.EsUserId,
                EsUsers = Convert(item.EsUsers),
                MemberId = item.MemberId,
                EsMembers = MembersManager.Convert(item.EsMembers),
                MemberRoleId = item.MemberRoleId,
                MembersRoles = MembersManager.Convert(item.MembersRoles)
            };
        }
        private static MemberUsersRolesModel Convert(MemberUsersRoles item)
        {
            if (item == null) return null;
            return new MemberUsersRolesModel
            {
                Id = item.Id,
                EsUserId = item.EsUserId,
                EsUsers = Convert(item.EsUsers),
                MemberId = item.MemberId,
                EsMembers = MembersManager.Convert(item.EsMembers),
                MemberRoleId = item.MemberRoleId,
                MembersRoles = MembersManager.Convert(item.MembersRoles)
            };
        }
        #endregion Converters

        private static string EncodePassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = Encoding.Default.GetBytes(password);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }
        private static List<EsUsers> TryGetEsUsers(bool? isActive)
        {
            using (var db = GetDataContext())
            {
                try
                {
                    return db.EsUsers.Where(s => isActive == null || s.IsActive == isActive.Value).ToList();
                }
                catch (Exception)
                {
                    return new List<EsUsers>();
                }
            }
        }
        private static bool TryEditUser(EsUsers user)
        {
            using (var db = GetDataContext())
            {
                try
                {
                    var exUser = db.EsUsers.SingleOrDefault(s => s.UserId == user.UserId ||
                        s.UserName.ToLower() == user.UserName.ToLower() ||
                        s.Email.ToLower() == user.Email.ToLower());
                    user.LastActivityDate = DateTime.Now;
                    if (exUser != null && user.UserId != 0)
                    {
                        exUser.UserId = user.UserId;
                        exUser.IsActive = user.IsActive;
                        exUser.UserName = user.UserName;
                        exUser.Email = user.Email;
                        exUser.Mobile = user.Mobile;
                        exUser.ClubSixteenId = user.ClubSixteenId;
                        exUser.LastActivityDate = user.LastActivityDate;
                    }
                    else
                    {
                        if (user.UserId != 0 || string.IsNullOrEmpty(user.Password)) return false;
                        user.UserId = db.EsUsers.Max(s => s.UserId) + 1;
                        db.EsUsers.Add(user);
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

        private static List<MemberUsersRoles> TryGetMemberUsersRoles(long memberId)
        {
            using (var db = GetDataContext())
            {
                try
                {
                    return db.MemberUsersRoles.Where(s => s.MemberId == memberId).Include(s=>s.MembersRoles).Include(s=>s.EsUsers).Include(s=>s.EsMembers).ToList();
                }
                catch (Exception)
                {
                    return new List<MemberUsersRoles>();
                }
            }
        }

        private static bool TryEditUserRoles(long userId, List<long> roleIds, long memberId)
        {
            using (var db = GetDataContext())
            {
                try
                {
                    var exRoles = db.MemberUsersRoles.Where(s => s.MemberId == memberId && s.EsUserId == userId);
                    foreach (var item in exRoles)
                    {
                        if (roleIds.Any(s => s == item.MemberRoleId))
                        {
                            roleIds.Remove(roleIds.First(s => s == item.MemberRoleId));
                        }
                        else
                        {
                            db.MemberUsersRoles.Remove(item);
                        }
                    }
                    foreach (var item in roleIds)
                    {
                        db.MemberUsersRoles.Add(new MemberUsersRoles()
                        {
                            Id = Guid.NewGuid(),
                            MemberId = memberId,
                            EsUserId = userId,
                            MemberRoleId = item
                        });
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
        #endregion Private methods

        #region Public methods
        public static List<EsUserModel> GetEsUsers(bool? isActive = null)
        {
            return TryGetEsUsers(isActive).Select(Convert).ToList();
        }
        public static bool EditUser(EsUserModel user)
        {
            return TryEditUser(Convert(user));
        }

        public static List<MemberUsersRolesModel> GetMemberUsersRoles(long memberId)
        {
            return TryGetMemberUsersRoles(memberId).Select(Convert).ToList();
        }
        public static bool EditUserRoles(long userId, IEnumerable<long> roleIds, long memberId)
        {
            return TryEditUserRoles(userId, roleIds.ToList(), memberId);
        }
        #endregion Public methods

        
    }
}
