using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;

namespace HBKPlatform.Repository;

public interface IClinicRepository
{
    public Task<ClinicDetailsDto> GetClinicAlone(int clinicId);
    public Task<List<ClinicDetailsLite>> GetClinicDetailsLite();
    public Task UpdateClinicDetails(ClinicDto clinic);
    public Task RegisterClinic(ClinicRegistrationDto clinic);
    public Task<bool> IsEmailInUse(string newEmail, string? currentEmail = null);
}