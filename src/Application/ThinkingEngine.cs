using System.Collections.Concurrent;
using System.Text.Json;
using Cogitare.Domain;

namespace Cogitare.Application;

internal class ThinkingEngine()
{
    private readonly ConcurrentBag<ThoughtData> _thoughtHistory = [];
    private readonly ConcurrentDictionary<string, ConcurrentBag<ThoughtData>> _branches = new();
    private int _thoughtCount;

    public Result<ThoughtResponse> ProcessThought(ThoughtData input)
    {
        var validationResult = ValidateThoughtData(input);
        if (validationResult is Failure<ThoughtData> failure)
            return Result.Failure<ThoughtResponse>(failure.Error);
        var adjustedInput = input switch
        {
            RegularThought rt => rt with { TotalThoughts = Math.Max(rt.TotalThoughts, rt.ThoughtNumber) },
            RevisionThought rev => rev with { TotalThoughts = Math.Max(rev.TotalThoughts, rev.ThoughtNumber) },
            BranchThought br => br with { TotalThoughts = Math.Max(br.TotalThoughts, br.ThoughtNumber) },
            _ => input
        };

        _thoughtHistory.Add(adjustedInput);
        Interlocked.Increment(ref _thoughtCount);
        if (adjustedInput is BranchThought branchThought)
        {
            _branches.AddOrUpdate(
                branchThought.BranchId,
                _ => new ConcurrentBag<ThoughtData> { adjustedInput },
                (_, existing) =>
                {
                    existing.Add(adjustedInput);
                    return existing;
                });
        }


        return new ThoughtResponse(
            adjustedInput.ThoughtNumber,
            adjustedInput.TotalThoughts,
            adjustedInput.NextThoughtNeeded,
            [.. _branches.Keys],
            _thoughtCount
        );
    }

    private static Result<ThoughtData> ValidateThoughtData(ThoughtData input)
    {
        if (string.IsNullOrWhiteSpace(input.Thought))
            return Result.Failure<ThoughtData>("Invalid thought: must be a non-empty string");

        if (input.ThoughtNumber < 1)
            return Result.Failure<ThoughtData>("Invalid thoughtNumber: must be a positive number");

        if (input.TotalThoughts < 1)
            return Result.Failure<ThoughtData>("Invalid totalThoughts: must be a positive number");

        return Result.Success(input);
    }

}