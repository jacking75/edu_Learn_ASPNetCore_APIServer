using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System;

namespace MatchAPIServer.Service;

public class MatchService : IMatchService
{
	readonly ILogger<MatchService> _logger;
	private readonly MatchWorker _matchWorker;

    public MatchService(ILogger<MatchService> logger, MatchWorker matchWorker)
	{
		_logger = logger;
		_matchWorker = matchWorker;
	}

	public bool AddUser(Int64 uid)
	{
		if (false == _matchWorker.AddUser(uid))
			return false;

		return true;
	}
}
