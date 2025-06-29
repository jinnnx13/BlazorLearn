using System.Data;
using Microsoft.Data.SqlClient;
using AgendaApp.Interfaces;
using AgendaApp.Models;

namespace Blazor1.Shared.Interface
{
    public interface IAgendaService
    {
        Task<IEnumerable<Agenda>> GetAllAgendasAsync();
        Task<IEnumerable<Agenda>> GetActiveAgendasAsync();
        Task<Agenda?> GetAgendaByIdAsync(int id);
        Task<Agenda> CreateAgendaAsync(Agenda agenda);
        Task<Agenda> UpdateAgendaAsync(Agenda agenda);
        Task<bool> DeleteAgendaAsync(int id, string deletedBy);
        Task<bool> ValidateAgendaTimeAsync(DateTime startTime, DateTime endTime, int? excludeId = null);
    }
}
