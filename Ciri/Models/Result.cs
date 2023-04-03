using Discord.Commands;

namespace Ciri.Models;

public class Result : RuntimeResult
{
	public Result(CommandError? error, string reason) : base(error, reason)
	{
		
	}
}