namespace Es.Data.Models
{
    public class MemberUsersRolesModel
    {
        public System.Guid Id { get; set; }
        public long MemberRoleId { get; set; }
        public long EsUserId { get; set; }
        public long MemberId { get; set; }

        public virtual EsMemberModel EsMembers { get; set; }
        public virtual EsUserModel EsUsers { get; set; }
        public virtual MemberRoleModel MembersRoles { get; set; }
    }
}
