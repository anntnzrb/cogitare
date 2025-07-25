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
        var adjustedInput = input with { TotalThoughts = Math.Max(input.TotalThoughts, input.ThoughtNumber) };

        _thoughtHistory.Add(adjustedInput);
        Interlocked.Increment(ref _thoughtCount);
        if (adjustedInput.IsBranch)
        {
            _branches.AddOrUpdate(
                adjustedInput.BranchId!,
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

    private static Result<ThoughtData> ValidateThoughtData(ThoughtData input) => input switch
    {
        { Thought: var t } when string.IsNullOrWhiteSpace(t) => Result.Failure<ThoughtData>("Invalid thought: must be a non-empty string"),
        { ThoughtNumber: var n } when n < 1 => Result.Failure<ThoughtData>("Invalid thoughtNumber: must be a positive number"),
        { TotalThoughts: var tt } when tt < 1 => Result.Failure<ThoughtData>("Invalid totalThoughts: must be a positive number"),
        _ => Result.Success(input)
    };

}