using Hbk.Platform.Database;
using Hbk.Platform.Exceptions;
using Hbk.Platform.Globals;
using Hbk.Platform.Models.DTO;
using Microsoft.EntityFrameworkCore;
using MissingFieldException = System.MissingFieldException;

namespace Hbk.Platform.Repository.Implementation
{
    /// <summary>
    /// Hbk.Platform Treatment repository
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
                Cost = x.Cost,
                Requestability = x.TreatmentRequestability,
                Img = x.Img
            }).FirstOrDefaultAsync() ?? throw new IdxNotFoundException("Could not find treatment Id");
        }
    
        public async Task<List<TreatmentLite>> GetTreatmentsLite(bool clientOnly)
        {
            var query = clientOnly ? _db.Treatments.Where(x => x.TreatmentRequestability == Enums.TreatmentRequestability.ClientAndPrac) : _db.Treatments;
            return await query.Select(x => new TreatmentLite()
            {
                Id = x.Id, Cost = x.Cost, Requestability = x.TreatmentRequestability, Title = x.Title
            }).ToListAsync();
        }

        public async Task CreateTreatment(TreatmentDto treatmentDto)
        {
            var treatment = new Treatment()
            {
                Title = treatmentDto.Title,
                Description = treatmentDto.Description,
                Cost = treatmentDto.Cost,
                TreatmentRequestability = treatmentDto.Requestability,
                Img = treatmentDto.Img
            };
            await _db.AddAsync(treatment);
            await _db.SaveChangesAsync();
        }
    
        public async Task UpdateTreatment(TreatmentDto treatmentDto)
        {
            if (treatmentDto == null) throw new MissingFieldException("Treatment missing, cannot proceed.");
            var treatment = await _db.Treatments.FirstOrDefaultAsync(x => x.Id == treatmentDto.Id) ??
                            throw new IdxNotFoundException($"Treatment {treatmentDto.Id} does not exist.");
            treatment.Title = treatmentDto.Title;
            treatment.Description = treatmentDto.Description;
            treatment.Cost = treatmentDto.Cost;
            treatment.TreatmentRequestability = treatmentDto.Requestability;
            treatment.Img = treatmentDto.Img;
            await _db.SaveChangesAsync();
        }
    
        // TODO: revisit, should we retain 'deleted' for a period of time?
        public async Task Delete(int treatmentId)
        {
            await _db.Treatments.Where(x => x.Id == treatmentId).ExecuteDeleteAsync();
        }
    }
}
