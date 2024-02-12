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
    public async Task<FullClientRecordDto> GetRecord(int recordId)
    {
        return await _db.ClientRecords.Where(x => x.Id == recordId).Select(x =>  SelectDto(x))
            .AsNoTracking().FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Record Id {recordId} not found.");
    }

    public async Task<List<ClientRecordLite>> GetClientRecordsLite(int clientId)
    {
        return await _db.ClientRecords.Where(x => x.ClientId == clientId).Select(x => new ClientRecordLite()
        {
            Id = x.Id, Date = x.DateCreated, Title = x.Title, Visibility = x.RecordVisibility, IsPriority = x.IsPriority
        }).ToListAsync();
    }

    public async Task<FullClientRecordDto> UpdateRecord(FullClientRecordDto recordDto)
    {
        var dbRecord = await _db.ClientRecords.FirstOrDefaultAsync(x => x.Id == recordDto.Id) ?? 
                       throw new InvalidOperationException($"Record Id {recordDto.Id} not found.");
        dbRecord.Title = recordDto.Title;
        dbRecord.NoteBody = recordDto.NoteBody;
        dbRecord.RecordVisibility = recordDto.Visibility;
        dbRecord.IsPriority = recordDto.IsPriority;
        await _db.SaveChangesAsync();
        return SelectDto(dbRecord);
    }
    
    public async Task<FullClientRecordDto> UpdateRecordLite(UpdateRecordLite recordDto)
    {
        var dbRecord = await _db.ClientRecords.FirstOrDefaultAsync(x => x.Id == recordDto.Id) ?? 
                       throw new InvalidOperationException($"Record Id {recordDto.Id} not found.");
        dbRecord.Title = recordDto.NoteTitle;
        dbRecord.NoteBody = recordDto.NoteBody;
        await _db.SaveChangesAsync();
        return SelectDto(dbRecord);
    }
    
    public async Task SetRecordPriority(int recordId, bool priority)
    {
        await _db.ClientRecords.Where(x => x.Id == recordId) 
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsPriority, priority));
    }

    public async Task<FullClientRecordDto> CreateRecord(ClientRecordDto recordDto)
    {
        var dbClientRecord = new ClientRecord()
        {
            ClientId = recordDto.ClientId,
            ClinicId = recordDto.ClinicId,
            PractitionerId = recordDto.PractitionerId,
            RecordVisibility = recordDto.Visibility,
            Title = recordDto.Title,
            NoteBody = recordDto.NoteBody,
            AppointmentId = recordDto.AppointmentId
        };
        await _db.AddAsync(dbClientRecord);
        await _db.SaveChangesAsync();
        return SelectDto(dbClientRecord);
    }

    // TODO: revisit, should we retain 'deleted' for a period of time?
    public async Task DeleteRecord(int recordId)
    {
        await _db.ClientRecords.Where(x => x.Id == recordId).ExecuteDeleteAsync();
    }

    private static FullClientRecordDto SelectDto(ClientRecord record)
    {
        return new FullClientRecordDto()
        {
            Id = record.Id,
            Title = record.Title,
            NoteBody = record.NoteBody,
            IsPriority = record.IsPriority,
            Visibility = record.RecordVisibility,
            AppointmentDate = record.Appointment?.DateCreated,
            AppointmentId = record.AppointmentId,
            ClientId = record.ClientId,
            DateCreated = record.DateCreated,
            DateUpdated = record.DateModified
        };
    }

}