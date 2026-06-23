namespace Hbk.Platform.Services.Implementation
{
    /// <summary>
    /// Hbk.Platform Tenancy service.
    /// "Hey, you gotta pay your dues before you pay the rent". S. M.
    /// 
    /// Author: Mark Brown
    /// Authored: 19/02/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class TenancyService : ITenancyService
    {
        public int TenancyId { get; private set; } = -1;
        public void SetTenancyId(int tenancyId)
        {
            TenancyId = tenancyId;
        }
    }
}