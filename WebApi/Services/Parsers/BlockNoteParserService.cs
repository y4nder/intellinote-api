using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Data.Entities;

namespace WebApi.Services.Parsers;

public class BlockNoteParserService
{
    private readonly ILogger<BlockNoteParserService> _logger;

    public BlockNoteParserService(ILogger<BlockNoteParserService> logger)
    {
        _logger = logger;
    }

    public string? PrepareNoteForEmbedding(Note note)
    {
        TryParse(note.Content, out var blocks);

        if (!blocks.Any())
        {
            return null;
        }
        
        var noteContent = Stringify(blocks);
        
        var text = $"{note.Title} {note.Summary}";
        if (note.Topics.Any())
        {
            text += string.Join(' ', note.Topics);
        }

        if (note.Keywords.Any())
        {
            text += string.Join(' ', note.Keywords);
        }
        return TextCleaner.Clean(text);
    }

    public string? ExtractNoteBlockContents(Note note)
    {
        TryParse(note.Content, out var blocks);
        
        if (!blocks.Any())
        {
            return null;
        }
        
        var noteContent = Stringify(blocks);
        return TextCleaner.Clean($"{note.Title} {noteContent}");
    }
    
    public void TryParse(string text, out List<PartialBlock> blocks)
    {
        blocks = new();
        try
        {
            blocks = JsonConvert.DeserializeObject<List<PartialBlock>>(text)!;
        } 
        catch (JsonException e)
        {
            _logger.LogError(e, "Error in TryParse");
        }
    }

    public string Stringify(List<PartialBlock> blocks)
    {
        var text = new StringBuilder();

        foreach (var block in blocks)
        {
            foreach (var content in block.Content)
            {
                switch (content)
                {
                    case InlineContent inline:
                        text.Append(inline.Text + " ");
                        break;

                    case TableContent table:
                        foreach (var row in table.Rows)
                        {
                            foreach (var cell in row.Cells)
                            {
                                foreach (var inline in cell.Content)
                                {
                                    text.Append(inline.Text + " ");
                                }
                            }
                        }
                        break;
                }
            }

            if (block.Children.Any())
            {
                text.Append(Stringify(block.Children));
            }
        }

        return TextCleaner.Clean(text.ToString());
    }
}

public class BlockContentConverter : JsonConverter<IBlockContent>
{
    public override IBlockContent ReadJson(JsonReader reader, Type objectType, IBlockContent existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        string type = obj["type"]?.ToString()?.ToLower()!;

        return (type switch
        {
            "table" => obj.ToObject<TableContent>(serializer),
            _ => obj.ToObject<InlineContent>(serializer)
        })!;
    }

    public override void WriteJson(JsonWriter writer, IBlockContent? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

public class BlockContentListConverter : JsonConverter<List<IBlockContent>>
{
    public override List<IBlockContent> ReadJson(JsonReader reader, Type objectType, List<IBlockContent> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var result = new List<IBlockContent>();
        
        if (reader.TokenType == JsonToken.StartArray)
        {
            var array = JArray.Load(reader);
            foreach (var item in array)
            {
                result.Add(DeserializeItem(item, serializer));
            }
        }
        else if (reader.TokenType == JsonToken.StartObject)
        {
            var obj = JObject.Load(reader);
            result.Add(DeserializeItem(obj, serializer));
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, List<IBlockContent> value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
    
    private IBlockContent DeserializeItem(JToken item, JsonSerializer serializer)
    {
        var type = item["type"]?.ToString()?.ToLower();
        return (type switch
        {
            "tablecontent" => item.ToObject<TableContent>(serializer),
            "tablecell" => item.ToObject<TableCellContent>(serializer), 
            _ => item.ToObject<InlineContent>(serializer)
        })!;
    }
}


public class PartialBlock
{
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    [JsonConverter(typeof(BlockContentListConverter))]
    public List<IBlockContent> Content { get; set; } = new();
    public List<PartialBlock> Children { get; set; } = new();
}

public interface IBlockContent
{
    public string Type { get; set; }
}

public class TableContent : IBlockContent
{
    public string Type { get; set; } = null!;
    public List<TableRowContent> Rows { get; set; } = new();
}

public class TableRowContent
{
    public List<TableCellContent> Cells { get; set; } = new();
}

public class TableCellContent : IBlockContent
{
    public string Type { get; set; } = null!;
    public List<InlineContent> Content { get; set; } = null!;
}

public class InlineContent : IBlockContent
{
    public string Type { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
}