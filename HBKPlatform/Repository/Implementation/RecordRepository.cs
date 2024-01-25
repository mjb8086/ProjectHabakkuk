using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// HBKPlatform Client record repository.
/// Database operations for client records.
/// 
/// Author: Mark Brown
/// Authored: 05/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class RecordRepository(ApplicationDbContext _db) : IRecordRepository
{
    public async Task<ClientRecordDto> GetRecord(int recordId)
    {
        return await _db.ClientRecords.Where(x => x.Id == recordId).Select(x => new ClientRecordDto()
        {
            Id = x.Id,  DateCreated = x.DateCreated, Title = x.Title, DateUpdated = x.DateModified, 
            Visibility = x.RecordVisibility, NoteBody = x.NoteBody, AppointmentId = x.AppointmentId,
            IsPriority = x.IsPriority, ClientId = x.ClientId, ClinicId = x.ClinicId
        }).AsNoTracking().FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Record Id {recordId} not found.");
    }

    public async Task<List<ClientRecordLite>> GetClientRecordsLite(int clientId)
    {
        return await _db.ClientRecords.Where(x => x.ClientId == clientId).Select(x => new ClientRecordLite()
        {
            Id = x.Id, Date = x.DateCreated, Title = x.Title, Visibility = x.RecordVisibility, IsPriority = x.IsPriority
        }).ToListAsync();
    }

    /*
    public async Task UpdateRecord(ClientRecordDto recordDto)
    {
        var dbRecord = await _db.ClientRecordList.FirstOrDefaultAsync(x => x.Id == recordDto.Id) ?? 
                       throw new InvalidOperationException($"Record Id {recordDto.Id} not found.");
        dbRecord.Title = recordDto.Title;
        dbRecord.NoteBody = recordDto.NoteBody;
        dbRecord.RecordVisibility = recordDto.Visibility;
        dbRecord.IsPriority = recordDto.IsPriority;
        await _db.SaveChangesAsync();
    }
    */
    
    public async Task<string> UpdateRecordBody(int recordId, string noteBody)
    {
        if (noteBody == null) throw new InvalidOperationException("Note body was null, cannot proceed.");
        var clientRecord = await _db.ClientRecords.FirstOrDefaultAsync(x => x.Id == recordId) ??
                           throw new InvalidOperationException($"Client record {recordId} does not exist.");
        clientRecord.NoteBody = noteBody;
        await _db.SaveChangesAsync();
        return clientRecord.NoteBody;
    }
    public async Task SetRecordPriority(int recordId, bool priority)
    {
        await _db.ClientRecords.Where(x => x.Id == recordId) 
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsPriority, priority));
    }

    public async Task CreateRecord(ClientRecordDto recordDto)
    {
        var dbClientRecord = new ClientRecord()
        {
            ClientId = recordDto.ClientId,
            ClinicId = recordDto.ClinicId,
            RecordVisibility = recordDto.Visibility,
            Title = recordDto.Title,
            NoteBody = recordDto.NoteBody,
            AppointmentId = recordDto.AppointmentId
        };
        await _db.AddAsync(dbClientRecord);
        await _db.SaveChangesAsync();
    }

    // TODO: revisit, should we retain 'deleted' for a period of time?
    public async Task DeleteRecord(int recordId)
    {
        await _db.ClientRecords.Where(x => x.Id == recordId).ExecuteDeleteAsync();
    }
    
}