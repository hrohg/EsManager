namespace Es.Data.Models
{
    public class MemberUsersRolesModel
    {
        public System.Guid Id { get; set; }
        public int MemberRoleId { get; set; }
        public int EsUserId { get; set; }
        public int MemberId { get; set; }

        public virtual EsMemberModel EsMembers { get; set; }
        public virtual EsUserModel EsUsers { get; set; }
        public virtual MemberRoleModel MembersRoles { get; set; }
    }
}
