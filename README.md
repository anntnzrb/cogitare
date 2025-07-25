# Cogitare MCP Server

An MCP server implementation that provides a tool for dynamic and reflective problem-solving through thoughts. This tool helps analyze problems through a flexible thinking process that can adapt and evolve.

## Features

- Break down complex problems into manageable thinking steps
- Revise and refine thoughts as understanding deepens
- Branch into alternative paths of reasoning
- Adjust the total number of thoughts dynamically
- Track progress through numbered thinking sequences

## Tool

### `think`

A detailed tool for dynamic and reflective problem-solving through thoughts. Each thought can build on, question, or revise previous insights as understanding deepens.

**Inputs:**
- `thought` (string): Your current thinking step
- `nextThoughtNeeded` (boolean): Whether another thought step is needed
- `thoughtNumber` (integer): Current thought number
- `totalThoughts` (integer): Estimated total thoughts needed
- `isRevision` (boolean, optional): Whether this revises previous thinking
- `revisesThought` (integer, optional): Which thought is being reconsidered
- `branchFromThought` (integer, optional): Branching point thought number
- `branchId` (string, optional): Branch identifier
- `needsMoreThoughts` (boolean, optional): If more thoughts are needed

## Usage

The think tool is designed for:
- Breaking down complex problems into steps
- Planning and analysis with room for revision
- Situations where the full scope might not be clear initially
- Tasks that need to maintain context over multiple thinking steps
- Problems where alternative approaches need exploration
- Analysis that might need course correction

## Configuration

## Docker

```json
{
  "mcpServers": {
    "cogitare": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "-i",
        "--init",
        "ghcr.io/anntnzrb/cogitare:latest"
      ]
    }
  }
}
```


## License

This MCP server is licensed under the MIT License. This means you are free to use, modify, and distribute the software, subject to the terms and conditions of the MIT License. For more details, please see the [COPYING](./COPYING) file in the project repository.
