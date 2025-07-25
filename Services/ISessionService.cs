using PRASARNET.Areas.MasterMng.Models;

namespace PRASARNET.Services
{
    public interface ISessionService
    {
        Task SetEmployeeSessionAsync(EmployeePersonalDetails employee);
        string? GetEmployeeEmail();
        string? GetEmployeeCode();
        int? GetEmployeeId();
        string? GetEmployeeName();
        string? GetEmployeeIP();
        void ClearSession();
        bool IsSessionValid();
    }
}
