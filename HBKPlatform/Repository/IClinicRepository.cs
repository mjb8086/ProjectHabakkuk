using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;

namespace HBKPlatform.Repository;

public interface IClinicRepository
{
    public Task<ClinicDto> GetClinicAlone(int clinicId);
    public Task<List<ClinicDetailsLite>> GetClinicDetailsLite();
    public Task UpdateClinicDetails(ClinicDto clinic);
}