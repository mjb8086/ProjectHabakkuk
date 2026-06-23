namespace Hbk.Platform.Services
{
    public interface ITenancyService
    {
        public int TenancyId { get; }
        public void SetTenancyId(int tenancyId);
    }
}