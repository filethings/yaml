using System.Text.RegularExpressions;

namespace FileThings.Yaml;

public sealed class YamlReader
{
    public Dictionary<string, object> ReadFromString(string yaml)
    {
        if (yaml is null)
        {
            throw new ArgumentNullException(nameof(yaml));
        }

        var yamlLines = yaml.Split('\n');

        var result = new Dictionary<string, object>();
        var currentIndentation = -1;
        var stack = new Stack<Dictionary<string, object>>();

        foreach (var line in yamlLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var indentation = CalculateIndentation(line);

            if (indentation > currentIndentation)
            {
                // Start a new mapping or sequence
                var key = line.TrimEnd(':');
                var value = new Dictionary<string, object>();
                stack.Push(value);

                if (result.Count == 0)
                {
                    // The root level
                    result[key] = value;
                }
                else
                {
                    var parentKey = stack.ElementAt(stack.Count - 2).Keys.Last();
                    stack.ElementAt(stack.Count - 2)[parentKey] = new Dictionary<string, object> { { key, value } };
                }

                currentIndentation = indentation;
            }
            else if (indentation == currentIndentation)
            {
                // Add scalar value to the current mapping
                var parts = line.Split(':');
                var key = parts[0].Trim();
                var value = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                stack.Peek()[key] = value;
            }
            else
            {
                // Move back to the previous indentation level
                while (stack.Count > 1 && indentation < currentIndentation)
                {
                    stack.Pop();
                    currentIndentation--;
                }
            }
        }

        return result;
    }

    private int CalculateIndentation(string line)
    {
        var match = Regex.Match(line, "^\\s*");
        return match.Length;
    }
}