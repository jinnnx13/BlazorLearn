using Blazor1.Shared.Interface;
using Blazor1.Models;

using System.Data;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.HttpResults;
using static MudBlazor.CategoryTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

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



--SQL Server Database Schema and Stored Procedures

-- Create the Agenda table
CREATE TABLE Agendas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StartTime DATETIME2 NOT NULL,
EndTime DATETIME2 NOT NULL,
    Agenda NVARCHAR(500) NOT NULL,
    Active BIT NOT NULL DEFAULT 1,
    InsertBy NVARCHAR(100) NOT NULL,
    InsertDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdateBy NVARCHAR(100) NULL,
    UpdateDate DATETIME2 NULL
);

--Create index for better performance
CREATE INDEX IX_Agendas_StartTime_EndTime ON Agendas(StartTime, EndTime);
CREATE INDEX IX_Agendas_Active ON Agendas(Active);

--Stored Procedure: Get all agendas
CREATE OR ALTER PROCEDURE sp_GetAllAgendas
AS
BEGIN
    SET NOCOUNT ON;

SELECT Id, StartTime, EndTime, Agenda, Active,
       InsertBy, InsertDate, UpdateBy, UpdateDate
    FROM Agendas
    ORDER BY StartTime;
END
GO

-- Stored Procedure: Get active agendas only
CREATE OR ALTER PROCEDURE sp_GetActiveAgendas
AS
BEGIN
    SET NOCOUNT ON;

SELECT Id, StartTime, EndTime, Agenda, Active,
       InsertBy, InsertDate, UpdateBy, UpdateDate
    FROM Agendas
    WHERE Active = 1
    ORDER BY StartTime;
END
GO

-- Stored Procedure: Get agenda by ID
CREATE OR ALTER PROCEDURE sp_GetAgendaById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

SELECT Id, StartTime, EndTime, Agenda, Active,
       InsertBy, InsertDate, UpdateBy, UpdateDate
    FROM Agendas
    WHERE Id = @Id;
END
GO

-- Stored Procedure: Create new agenda
CREATE OR ALTER PROCEDURE sp_CreateAgenda
    @StartTime DATETIME2,
    @EndTime DATETIME2,
    @Agenda NVARCHAR(500),
    @InsertBy NVARCHAR(100),
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

--Validate time range
    IF @StartTime >= @EndTime
    BEGIN
        RAISERROR('Start time must be before end time', 16, 1);
RETURN;
END

-- Check for time conflicts with active agendas
    IF EXISTS (
        SELECT 1 FROM Agendas 
        WHERE Active = 1 
        AND (@StartTime < EndTime AND @EndTime > StartTime)
    )
    BEGIN
        RAISERROR('The selected time conflicts with an existing agenda', 16, 1);
RETURN;
END

INSERT INTO Agendas (StartTime, EndTime, Agenda, InsertBy, InsertDate, Active)
    VALUES (@StartTime, @EndTime, @Agenda, @InsertBy, GETDATE(), 1);

SET @NewId = SCOPE_IDENTITY();
END
GO

-- Stored Procedure: Update agenda
CREATE OR ALTER PROCEDURE sp_UpdateAgenda
    @Id INT,
    @StartTime DATETIME2,
    @EndTime DATETIME2,
    @Agenda NVARCHAR(500),
    @UpdateBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

--Check if agenda exists
IF NOT EXISTS(SELECT 1 FROM Agendas WHERE Id = @Id)
    BEGIN
        RAISERROR('Agenda not found', 16, 1);
RETURN;
END

-- Validate time range
    IF @StartTime >= @EndTime
    BEGIN
        RAISERROR('Start time must be before end time', 16, 1);
RETURN;
END

-- Check for time conflicts with active agendas (excluding current record)
    IF EXISTS (
        SELECT 1 FROM Agendas 
        WHERE Active = 1 
        AND Id != @Id
        AND (@StartTime < EndTime AND @EndTime > StartTime)
    )
    BEGIN
        RAISERROR('The selected time conflicts with an existing agenda', 16, 1);
RETURN;
END

UPDATE Agendas 
    SET StartTime = @StartTime,
    EndTime = @EndTime,
    Agenda = @Agenda,
UpdateBy = @UpdateBy,
    UpdateDate = GETDATE()
    WHERE Id = @Id;
END
GO

-- Stored Procedure: Soft delete agenda
CREATE OR ALTER PROCEDURE sp_DeleteAgenda
    @Id INT,
    @DeletedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
--Check if agenda exists
IF NOT EXISTS(SELECT 1 FROM Agendas WHERE Id = @Id)
    BEGIN
        RAISERROR('Agenda not found', 16, 1);
RETURN;
END

UPDATE Agendas 
    SET Active = 0,
    UpdateBy = @DeletedBy,
    UpdateDate = GETDATE()
    WHERE Id = @Id;
END
GO

-- Stored Procedure: Validate agenda time (for client-side validation)
CREATE OR ALTER PROCEDURE sp_ValidateAgendaTime
    @StartTime DATETIME2,
    @EndTime DATETIME2,
    @ExcludeId INT = NULL,
    @IsValid BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
SET @IsValid = 1;

--Validate time range
    IF @StartTime >= @EndTime
    BEGIN
        SET @IsValid = 0;
RETURN;
END

-- Check for time conflicts
    IF EXISTS (
        SELECT 1 FROM Agendas 
        WHERE Active = 1 
        AND (@ExcludeId IS NULL OR Id != @ExcludeId)
        AND (@StartTime < EndTime AND @EndTime > StartTime)
    )
    BEGIN
        SET @IsValid = 0;
END
END
GO
