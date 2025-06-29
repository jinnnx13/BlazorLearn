using Blazor1.Shared.Interface;
using Blazor1.Models;

using System.Data;

namespace Blazor1.Shared.Data
{
    public class AgendaRepository : IAgendaRepository
    {
        private readonly string _connectionString;

        public AgendaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<Agenda>> GetAllAgendasAsync()
        {
            var agendas = new List<Agenda>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllAgendas", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                agendas.Add(MapReaderToAgenda(reader));
            }

            return agendas;
        }

        public async Task<IEnumerable<Agenda>> GetActiveAgendasAsync()
        {
            var agendas = new List<Agenda>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetActiveAgendas", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                agendas.Add(MapReaderToAgenda(reader));
            }

            return agendas;
        }

        public async Task<Agenda?> GetAgendaByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAgendaById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapReaderToAgenda(reader);
            }

            return null;
        }

        public async Task<int> CreateAgendaAsync(Agenda agenda)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateAgenda", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartTime", agenda.StartTime);
            command.Parameters.AddWithValue("@EndTime", agenda.EndTime);
            command.Parameters.AddWithValue("@Agenda", agenda.AgendaItem);
            command.Parameters.AddWithValue("@InsertBy", agenda.InsertBy);

            var newIdParameter = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParameter);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (int)newIdParameter.Value;
        }

        public async Task UpdateAgendaAsync(Agenda agenda)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_UpdateAgenda", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Id", agenda.Id);
            command.Parameters.AddWithValue("@StartTime", agenda.StartTime);
            command.Parameters.AddWithValue("@EndTime", agenda.EndTime);
            command.Parameters.AddWithValue("@Agenda", agenda.AgendaItem);
            command.Parameters.AddWithValue("@UpdateBy", agenda.UpdateBy ?? (object)DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAgendaAsync(int id, string deletedBy)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_DeleteAgenda", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@DeletedBy", deletedBy);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> ValidateAgendaTimeAsync(DateTime startTime, DateTime endTime, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ValidateAgendaTime", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartTime", startTime);
            command.Parameters.AddWithValue("@EndTime", endTime);
            command.Parameters.AddWithValue("@ExcludeId", excludeId ?? (object)DBNull.Value);

            var isValidParameter = new SqlParameter("@IsValid", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(isValidParameter);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (bool)isValidParameter.Value;
        }

        private static Agenda MapReaderToAgenda(SqlDataReader reader)
        {
            return new Agenda
            {
                Id = reader.GetInt32("Id"),
                StartTime = reader.GetDateTime("StartTime"),
                EndTime = reader.GetDateTime("EndTime"),
                AgendaItem = reader.GetString("Agenda"),
                Active = reader.GetBoolean("Active"),
                InsertBy = reader.GetString("InsertBy"),
                InsertDate = reader.GetDateTime("InsertDate"),
                UpdateBy = reader.IsDBNull("UpdateBy") ? null : reader.GetString("UpdateBy"),
                UpdateDate = reader.IsDBNull("UpdateDate") ? null : reader.GetDateTime("UpdateDate")
            };
        }
    }
}
