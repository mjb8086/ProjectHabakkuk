namespace HBKPlatform.Services
{
    public interface ISecurityService
    {
        public Task<bool> VerifyClientPracOwnership(int clientId, int pracId);
        public void ClearClientPracOwnership();
    }
}