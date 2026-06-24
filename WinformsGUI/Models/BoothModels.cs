using System;
using System.Text.Json.Serialization;

namespace WinformsGUI.Models
{
    public class BoothResponse
    {
        [JsonPropertyName("boothId")]
        public Guid Id { get; set; }

        [JsonPropertyName("boothName")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("boothNumber")]
        public string Location { get; set; } = string.Empty;
    }

    public class CreateBoothRequest
    {
        [JsonPropertyName("boothName")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("boothNumber")]
        public string Location { get; set; } = string.Empty;
    }

    public class UpdateBoothRequest
    {
        [JsonPropertyName("boothName")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("boothNumber")]
        public string Location { get; set; } = string.Empty;
    }
}
