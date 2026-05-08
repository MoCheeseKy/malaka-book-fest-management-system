namespace MalakaBookFest.Infrastructure.Configuration;

/// <summary>
/// Maps the "TalkshowConfig" section of appsettings.json.
/// Controls registration windows and capacity defaults.
/// </summary>
public class TalkshowConfig
{
    public int DefaultMaxCapacity { get; set; } = 100;
    public int RegistrationOpenDaysBeforeEvent { get; set; } = 14;
}
