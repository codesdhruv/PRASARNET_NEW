using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PRASARNET.Areas.MasterMng.Models;
using System;
using System.Collections.Concurrent;

namespace PRASARNET.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SetEmployeeSessionAsync(EmployeePersonalDetails employee)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;
            _httpContextAccessor.HttpContext?.Session.SetString("SessionkeyEmail", employee.IT_NICEmail);
            _httpContextAccessor.HttpContext?.Session.SetString("SessionkeyCode", employee.EmployeeCode);
            _httpContextAccessor.HttpContext?.Session.SetInt32("SessionkeyId", employee.EmployeeId);
            _httpContextAccessor .HttpContext?.Session.SetString("SessionkeyName", employee.FirstName);
            _httpContextAccessor .HttpContext?.Session.SetString("SessionkeyIP", context.Connection?.RemoteIpAddress?.ToString() ?? "Unknown");
        }
        public string? GetEmployeeEmail() => _httpContextAccessor.HttpContext?.Session.GetString("SessionkeyEmail");
        public string? GetEmployeeCode() => _httpContextAccessor.HttpContext?.Session.GetString("SessionkeyCode");
        public int? GetEmployeeId() => _httpContextAccessor.HttpContext?.Session.GetInt32("SessionkeyId");
        public string? GetEmployeeName() => _httpContextAccessor.HttpContext?.Session.GetString("SessionkeyName");
        public string? GetEmployeeIP() => _httpContextAccessor.HttpContext?.Session.GetString("SessionkeyIP");
        public void ClearSession()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }
        public bool IsSessionValid()
        {
            return GetEmployeeId() != null;
        }
    }
}
