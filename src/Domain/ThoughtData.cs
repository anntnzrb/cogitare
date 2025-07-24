namespace Cogitare.Domain;

internal abstract record ThoughtData(
    string Thought,
    int ThoughtNumber,
    int TotalThoughts,
    bool NextThoughtNeeded
);

internal sealed record RegularThought(
    string Thought,
    int ThoughtNumber,
    int TotalThoughts,
    bool NextThoughtNeeded,
    bool NeedsMoreThoughts
) : ThoughtData(Thought, ThoughtNumber, TotalThoughts, NextThoughtNeeded);

internal sealed record RevisionThought(
    string Thought,
    int ThoughtNumber,
    int TotalThoughts,
    bool NextThoughtNeeded,
    int RevisesThought
) : ThoughtData(Thought, ThoughtNumber, TotalThoughts, NextThoughtNeeded);

internal sealed record BranchThought(
    string Thought,
    int ThoughtNumber,
    int TotalThoughts,
    bool NextThoughtNeeded,
    int BranchFromThought,
    string BranchId
) : ThoughtData(Thought, ThoughtNumber, TotalThoughts, NextThoughtNeeded);

internal record ThoughtResponse(
    int ThoughtNumber,
    int TotalThoughts,
    bool NextThoughtNeeded,
    string[] Branches,
    int ThoughtHistoryLength
);