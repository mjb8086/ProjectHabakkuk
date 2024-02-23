namespace HBKPlatform.Services;

public interface ITenancyService
{
    public int TenancyId { get; }
    public void SetTenancyId(int tenancyId);
}