namespace Ciri.Models;

public class ProfitData
{
	public long Hearts { get; init; }
	public ulong[] Members { get; set; }
	
	public ProfitData(long hearts, IEnumerable<ulong> members)
	{
		Hearts = hearts;
		Members = members.ToArray();
	}
}