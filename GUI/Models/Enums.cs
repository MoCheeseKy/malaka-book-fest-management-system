namespace GUI.Models
{
    public enum BoothCategory
    {
        Publisher,
        IndieAuthor,
        Merchandise,
        FoodBeverage,
        Other
    }

    public enum TalkshowStatus
    {
        Scheduled,
        Ongoing,
        Completed,
        Cancelled
    }

    public enum TicketStatus
    {
        Active,
        Used,
        Cancelled,
        Expired
    }

    public enum TicketType
    {
        SingleDay,
        AllAccess,
        VIP
    }
}
