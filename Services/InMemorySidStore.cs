namespace PRASARNET.Services
{
    using System.Collections.Concurrent;

    public class InMemorySidStore : ISidStore
    {
        private readonly ConcurrentDictionary<string, (string UserId, bool Invalidated)> _store = new();

        public Task SaveAsync(string sid, string userId)
        {
            _store[sid] = (userId, false); // false = not invalidated
            return Task.CompletedTask;
        }

        public Task<bool> IsSidInvalidatedAsync(string sid)
        {
            if (_store.TryGetValue(sid, out var entry))
            {
                return Task.FromResult(entry.Invalidated);
            }
            return Task.FromResult(false);
        }

        public Task InvalidateSidAsync(string sid)
        {
            if (_store.TryGetValue(sid, out var entry))
            {
                _store[sid] = (entry.UserId, true);
            }
            return Task.CompletedTask;
        }

        public Task<string?> GetUserIdBySidAsync(string sid)
        {
            if (_store.TryGetValue(sid, out var entry))
            {
                return Task.FromResult<string?>(entry.UserId);
            }
            return Task.FromResult<string?>(null);
        }
    }

}
