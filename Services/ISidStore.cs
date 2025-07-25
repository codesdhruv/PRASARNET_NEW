namespace PRASARNET.Services
{
    public interface ISidStore
    {
        Task SaveAsync(string sid, string username);
        Task<bool> IsSidInvalidatedAsync(string sid);
        Task InvalidateSidAsync(string sid);
    }
}