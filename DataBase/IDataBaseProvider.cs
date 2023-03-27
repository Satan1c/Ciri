using DataBase.Models;

namespace DataBase;

public interface IDataBaseProvider
{
	public Task SyncCache();
	public Task<bool> HasProfile(ulong id);
	public Task<Profile> GetProfile(ulong id, bool fetch = true);
	public Task AddOrUpdateProfile(Profile profile);
}