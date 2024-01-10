using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// HBKPlatform Treatment repository
/// 
/// Author: Mark Brown
/// Authored: 10/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>

public class TreatmentRepository(ApplicationDbContext _db) : ITreatmentRepository
{
    public async Task<TreatmentDto> GetTreatment(int treatmentId)
    {
        return await _db.Treatments.Where(x => x.Id == treatmentId).Select(x => new TreatmentDto()
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            ClinicId = x.ClinicId,
            Cost = x.Cost,
            Requestability = x.TreatmentRequestability,
            Img = x.Img
        }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Could not find treatment Id");
    }
    
    public async Task<List<TreatmentLite>> GetClinicTreatments(int clinicId)
    {
        var treatments = await _db.Treatments.Where(x => x.ClinicId == clinicId).Select(x => new TreatmentLite()
        {
            Id = x.Id, Cost = x.Cost, Requestability = x.TreatmentRequestability, Title = x.Title
        }).ToListAsync();
        return treatments != null  && treatments.Any() ? treatments : new List<TreatmentLite>();
    }

    public async Task CreateTreatment(TreatmentDto treatmentDto)
    {
        var treatment = new Treatment()
        {
            ClinicId = treatmentDto.ClinicId,
            Title = treatmentDto.Title,
            Description = treatmentDto.Description,
            Cost = treatmentDto.Cost,
            TreatmentRequestability = treatmentDto.Requestability,
            Img = treatmentDto.Img
        };
        await _db.AddAsync(treatment);
        await _db.SaveChangesAsync();
    }
    
}