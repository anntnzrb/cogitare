using System.Text.Json;
using Cogitare.Application;
using Cogitare.Domain;

namespace Cogitare.Infrastructure;

[McpServerToolType]
internal static class ThinkingTool
{
    private static readonly Lazy<ThinkingEngine> _engine = new(() => new ThinkingEngine());
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [McpServerTool(Name = "think"), Description("A detailed tool for dynamic and reflective problem-solving through thoughts. This tool helps analyze problems through a flexible thinking process that can adapt and evolve. Each thought can build on, question, or revise previous insights as understanding deepens.")]
    public static string Think(
        [Description("Your current thinking step")] string thought,
        [Description("Whether another thought step is needed")] bool nextThoughtNeeded,
        [Description("Current thought number")] int thoughtNumber,
        [Description("Estimated total thoughts needed")] int totalThoughts,
        [Description("Whether this revises previous thinking")] bool? isRevision = null,
        [Description("Which thought is being reconsidered")] int? revisesThought = null,
        [Description("Branching point thought number")] int? branchFromThought = null,
        [Description("Branch identifier")] string? branchId = null,
        [Description("If more thoughts are needed")] bool? needsMoreThoughts = null)
    {
        try
        {
            var thoughtData = CreateThoughtData(
                thought,
                thoughtNumber,
                totalThoughts,
                nextThoughtNeeded,
                isRevision,
                revisesThought,
                branchFromThought,
                branchId,
                needsMoreThoughts);

            var response = _engine.Value.ProcessThought(thoughtData);

            return response switch
            {
                Success<ThoughtResponse> success => JsonSerializer.Serialize(success.Value, _jsonOptions),
                Failure<ThoughtResponse> failure => throw new McpException(failure.Error),
                _ => throw new McpException("Unexpected result type")
            };
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new McpException($"Processing error: {ex.Message}");
        }
    }

    private static ThoughtData CreateThoughtData(
        string thought,
        int thoughtNumber,
        int totalThoughts,
        bool nextThoughtNeeded,
        bool? isRevision,
        int? revisesThought,
        int? branchFromThought,
        string? branchId,
        bool? needsMoreThoughts)
    {
        if (isRevision == true && revisesThought.HasValue)
        {
            return new RevisionThought(thought, thoughtNumber, totalThoughts, nextThoughtNeeded, revisesThought.Value);
        }

        if (branchFromThought.HasValue && !string.IsNullOrEmpty(branchId))
        {
            return new BranchThought(thought, thoughtNumber, totalThoughts, nextThoughtNeeded, branchFromThought.Value, branchId);
        }

        return new RegularThought(thought, thoughtNumber, totalThoughts, nextThoughtNeeded, needsMoreThoughts ?? true);
    }
}