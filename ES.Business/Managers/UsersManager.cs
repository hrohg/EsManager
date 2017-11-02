using System;
using System.Collections.Generic;
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
        private static EsUserModel Convert(EsUsers user)
        {
            return new EsUserModel
            {
                UserId = user.UserId,
                IsActive = user.IsActive,
                UserName = user.UserName,
                Email = user.Email,
                Mobile = user.Mobile,
                ClubSixteenId = user.ClubSixteenId,
                LastActivityDate = user.LastActivityDate
            };
        }
        private static EsUsers Convert(EsUserModel user)
        {
            return new EsUsers
            {
                UserId = user.UserId,
                IsActive = user.IsActive,
                UserName = user.UserName,
                Email = user.Email,
                Mobile = user.Mobile,
                ClubSixteenId = user.ClubSixteenId,
                LastActivityDate = user.LastActivityDate,
                Password = EncodePassword(user.NewPassword)
            };
        }
        #endregion Converters

        private static string EncodePassword(string password)
        {
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
                        string.Equals(s.UserName, user.UserName, StringComparison.CurrentCultureIgnoreCase) ||
                        string.Equals(s.Email, user.Email, StringComparison.CurrentCultureIgnoreCase));
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

        #endregion Public methods


    }
}
