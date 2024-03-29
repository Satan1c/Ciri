﻿namespace Ciri.Models;

public class ProfitData
{
	public ProfitData(long hearts, IEnumerable<ulong> members)
	{
		Hearts = hearts;
		Members = members.ToArray();
	}

	public long Hearts { get; init; }
	public ulong[] Members { get; set; }
}