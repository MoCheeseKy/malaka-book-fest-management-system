namespace WinformsGUI.Models
{
    public class TalkshowResponse
    {
        public Guid TalkshowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string SpeakerBio { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public int MaxCapacity { get; set; }
        public int RegisteredCount { get; set; }
    }

    public class CreateTalkshowRequest
    {
        public string Title { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string SpeakerBio { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }
    }

    public class UpdateTalkshowRequest
    {
        public string Title { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string SpeakerBio { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }
    }
}
