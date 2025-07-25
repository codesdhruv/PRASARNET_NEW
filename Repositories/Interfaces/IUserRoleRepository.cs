namespace PRASARNET.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<List<string>> GetRolesForUserAsync(string username);
        Task<List<string>> GetPermissionsForUserAsync(string username);
    }
}
