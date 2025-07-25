namespace Cogitare.Domain;

internal record ThoughtData(
    string Thought,
    int ThoughtNumber,
    int TotalThoughts,
    bool NextThoughtNeeded
)
{
    public bool? NeedsMoreThoughts { get; init; }
    public int? RevisesThought { get; init; }
    public int? BranchFromThought { get; init; }
    public string? BranchId { get; init; }

    public bool IsRegular => NeedsMoreThoughts.HasValue;
    public bool IsRevision => RevisesThought.HasValue;
    public bool IsBranch => BranchFromThought.HasValue && BranchId is not null;

    public static ThoughtData Regular(string thought, int thoughtNumber, int totalThoughts, bool nextThoughtNeeded, bool needsMoreThoughts) =>
        new(thought, thoughtNumber, totalThoughts, nextThoughtNeeded) { NeedsMoreThoughts = needsMoreThoughts };

    public static ThoughtData Revision(string thought, int thoughtNumber, int totalThoughts, bool nextThoughtNeeded, int revisesThought) =>
        new(thought, thoughtNumber, totalThoughts, nextThoughtNeeded) { RevisesThought = revisesThought };

    public static ThoughtData Branch(string thought, int thoughtNumber, int totalThoughts, bool nextThoughtNeeded, int branchFromThought, string branchId) =>
        new(thought, thoughtNumber, totalThoughts, nextThoughtNeeded) { BranchFromThought = branchFromThought, BranchId = branchId };
};

internal record ThoughtResponse(
    int ThoughtNumber,
    int TotalThoughts,
    bool NextThoughtNeeded,
    string[] Branches,
    int ThoughtHistoryLength
);