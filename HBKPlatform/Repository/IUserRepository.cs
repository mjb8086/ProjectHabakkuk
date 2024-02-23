namespace HBKPlatform.Repository;

public interface IUserRepository
{
    public Task ResetPasswordForUser(string userId);
    public Task ToggleLockout(string userId);
    public Task<bool> IsEmailInUse(string newEmail, string? currentEmail = null);

}