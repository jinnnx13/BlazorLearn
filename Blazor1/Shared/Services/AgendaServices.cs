using AgendaApp.Interfaces;
using AgendaApp.Models;
using Blazor1.Shared.Interface;

namespace Blazor1.Shared.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IAgendaRepository _repository;
        private readonly ILogger<AgendaService> _logger;

        public AgendaService(IAgendaRepository repository, ILogger<AgendaService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Agenda>> GetAllAgendasAsync()
        {
            try
            {
                return await _repository.GetAllAgendasAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all agendas");
                throw;
            }
        }

        public async Task<IEnumerable<Agenda>> GetActiveAgendasAsync()
        {
            try
            {
                return await _repository.GetActiveAgendasAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active agendas");
                throw;
            }
        }

        public async Task<Agenda?> GetAgendaByIdAsync(int id)
        {
            try
            {
                return await _repository.GetAgendaByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting agenda by id {Id}", id);
                throw;
            }
        }

        public async Task<Agenda> CreateAgendaAsync(Agenda agenda)
        {
            try
            {
                var newId = await _repository.CreateAgendaAsync(agenda);
                agenda.Id = newId;
                return agenda;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating agenda");
                throw;
            }
        }

        public async Task<Agenda> UpdateAgendaAsync(Agenda agenda)
        {
            try
            {
                await _repository.UpdateAgendaAsync(agenda);
                return agenda;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agenda {Id}", agenda.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAgendaAsync(int id, string deletedBy)
        {
            try
            {
                await _repository.DeleteAgendaAsync(id, deletedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting agenda {Id}", id);
                return false;
            }
        }

        public async Task<bool> ValidateAgendaTimeAsync(DateTime startTime, DateTime endTime, int? excludeId = null)
        {
            try
            {
                return await _repository.ValidateAgendaTimeAsync(startTime, endTime, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating agenda time");
                return false;
            }
        }
    }
}
