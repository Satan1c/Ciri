using DataBase.Models;
using Discord.Interactions;

namespace Ciri.Models;

public class ProfileArg
{
	public string? Bio { get; set; }
	public long? Hearts { get; set; }
	public long? Reputation { get; set; }
	public ulong? Lover { get; set; }
	public ulong? Messages { get; set; }
			
	[ComplexParameterCtor]
	public ProfileArg(string? bio = null, long? hearts = null, long? reputation = null, ulong? lover = null, ulong? messages = null)
	{
		Bio = bio;
		Hearts = hearts;
		Reputation = reputation;
		Lover = lover;
		Messages = messages;
	}
	
	public Profile EditProfile(Profile profile)
	{
		if (Bio != null)
			profile.Bio = Bio;
		if (Hearts != null)
			profile.Hearts = Hearts.Value;
		if (Reputation != null)
			profile.Reputation = Reputation.Value;
		if (Lover != null)
			profile.Lover = Lover.Value;
		if (Messages != null)
			profile.Messages = Messages.Value;
		
		return profile;
	}
	
	public bool IsEmpty()
	{
		return Bio == null && Hearts == null && Reputation == null && Lover == null && Messages == null;
	}
}