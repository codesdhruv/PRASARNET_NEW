using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Models;

namespace PRASARNET.Repositories.Interfaces
{
    public interface IUserDataRepository
    {
        Task<EmployeePersonalDetails?> FetchEmployeeByEmailAsync(string emailId);
        Task<List<MenuItem>> FetchUserMenuByEmployeeIdAsync(int employeeId);
        Task<List<String>> GetPermissionsForUserAsync(string emailId);
        Task<List<String>> GetRolesForUserAsync(string emailId);
    }
}
