using AgendaApp.Models;

namespace Blazor1.Shared.Interface
{
    public interface IAgendaRepository
    {
        Task<IEnumerable<Agenda>> GetAllAgendasAsync();
        Task<IEnumerable<Agenda>> GetActiveAgendasAsync();
        Task<Agenda?> GetAgendaByIdAsync(int id);
        Task<int> CreateAgendaAsync(Agenda agenda);
        Task UpdateAgendaAsync(Agenda agenda);
        Task DeleteAgendaAsync(int id, string deletedBy);
        Task<bool> ValidateAgendaTimeAsync(DateTime startTime, DateTime endTime, int? excludeId = null);
    }
}