using System.Reflection;
using System.Text;

namespace FileThings.Yaml.UnitTests;

public class FileThingsReaderTests
{
    [Theory]
    [InlineData(null)]
    public void ReadFromString_ThrowsExceptionWhenStringIsNull(string inputYaml)
    {
        YamlReader yamlReader = new();
        Assert.Throws<ArgumentNullException>(() => yamlReader.ReadFromString(inputYaml));
    }

    [Fact]
    public async Task ReadFromString_Works()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceStream = assembly?.GetManifestResourceStream("FileThings.Yaml.UnitTests.Stubs.sample_01.yaml")
                                    ?? throw new InvalidOperationException("");
        using var reader = new StreamReader(resourceStream, Encoding.UTF8);
        string inputYaml = await reader.ReadToEndAsync();

        YamlReader yamlReader = new();
        var yamlObject = yamlReader.ReadFromString(inputYaml);
    }
}