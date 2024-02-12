namespace HBKPlatform.Repository;

public interface IUserRepository
{
    public Task ResetPasswordForUser(string userId);
    public Task ToggleLockout(string userId);

}