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

        [JsonPropertyName("category")]
        public int Category { get; set; }

        [JsonIgnore]
        public string CategoryName => Category switch
        {
            0 => "Publisher",
            1 => "Indie Author",
            2 => "Merchandise",
            3 => "Food & Beverage",
            _ => "Other"
        };

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }

    public class CreateBoothRequest
    {
        [JsonPropertyName("boothName")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("boothNumber")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public int Category { get; set; }
    }

    public class UpdateBoothRequest
    {
        [JsonPropertyName("boothName")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("boothNumber")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public int Category { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}
