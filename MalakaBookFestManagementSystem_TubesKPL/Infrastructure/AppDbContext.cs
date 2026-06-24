using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MalakaBookFest.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Booth> Booths => Set<Booth>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Talkshow> Talkshows => Set<Talkshow>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TalkshowRegistration> TalkshowRegistrations => Set<TalkshowRegistration>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.UserId);
            e.Property(u => u.UserId).HasColumnName("user_id");
            e.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            e.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();
            e.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
            e.Property(u => u.Role).HasColumnName("role");
            e.Property(u => u.IsActive).HasColumnName("is_active");
            e.Property(u => u.CreatedAt).HasColumnName("created_at");
            e.Property(u => u.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Booth>(e =>
        {
            e.ToTable("booths");
            e.HasKey(b => b.BoothId);
            e.Property(b => b.BoothId).HasColumnName("booth_id");
            e.Property(b => b.OrganizerId).HasColumnName("organizer_id");
            e.Property(b => b.BoothName).HasColumnName("booth_name").HasMaxLength(255).IsRequired();
            e.Property(b => b.Description).HasColumnName("description");
            e.Property(b => b.BoothNumber).HasColumnName("booth_number").HasMaxLength(20).IsRequired();
            e.Property(b => b.Category).HasColumnName("category");
            e.Property(b => b.IsActive).HasColumnName("is_active");
            e.Property(b => b.CreatedAt).HasColumnName("created_at");
            e.Property(b => b.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(b => b.BoothNumber).IsUnique();
            e.HasOne(b => b.Organizer).WithMany(u => u.ManagedBooths)
                .HasForeignKey(b => b.OrganizerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Book>(e =>
        {
            e.ToTable("books");
            e.HasKey(b => b.BookId);
            e.Property(b => b.BookId).HasColumnName("book_id");
            e.Property(b => b.BoothId).HasColumnName("booth_id");
            e.Property(b => b.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            e.Property(b => b.Author).HasColumnName("author").HasMaxLength(255).IsRequired();
            e.Property(b => b.Isbn).HasColumnName("isbn").HasMaxLength(20);
            e.Property(b => b.Price).HasColumnName("price").HasColumnType("decimal(12,2)");
            e.Property(b => b.Stock).HasColumnName("stock");
            e.Property(b => b.CoverUrl).HasColumnName("cover_url");
            e.Property(b => b.CreatedAt).HasColumnName("created_at");
            e.Property(b => b.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(b => b.Booth).WithMany(bt => bt.Books)
                .HasForeignKey(b => b.BoothId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Talkshow>(e =>
        {
            e.ToTable("talkshows");
            e.HasKey(t => t.TalkshowId);
            e.Property(t => t.TalkshowId).HasColumnName("talkshow_id");
            e.Property(t => t.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            e.Property(t => t.SpeakerName).HasColumnName("speaker_name").HasMaxLength(255).IsRequired();
            e.Property(t => t.SpeakerBio).HasColumnName("speaker_bio");
            e.Property(t => t.Venue).HasColumnName("venue").HasMaxLength(255).IsRequired();
            e.Property(t => t.StartTime).HasColumnName("start_time");
            e.Property(t => t.EndTime).HasColumnName("end_time");
            e.Property(t => t.MaxCapacity).HasColumnName("max_capacity");
            e.Property(t => t.Status).HasColumnName("status");
            e.Property(t => t.CreatedAt).HasColumnName("created_at");
            e.Property(t => t.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Ticket>(e =>
        {
            e.ToTable("tickets");
            e.HasKey(t => t.TicketId);
            e.Property(t => t.TicketId).HasColumnName("ticket_id");
            e.Property(t => t.UserId).HasColumnName("user_id");
            e.Property(t => t.Type).HasColumnName("type");
            e.Property(t => t.Status).HasColumnName("status");
            e.Property(t => t.QrCode).HasColumnName("qr_code").HasMaxLength(512);
            e.Property(t => t.PricePaid).HasColumnName("price_paid").HasColumnType("decimal(12,2)");
            e.Property(t => t.PurchasedAt).HasColumnName("purchased_at");
            e.Property(t => t.ValidDate).HasColumnName("valid_date");
            e.HasOne(t => t.User).WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TalkshowRegistration>(e =>
        {
            e.ToTable("talkshow_registrations");
            e.HasKey(r => r.RegistrationId);
            e.Property(r => r.RegistrationId).HasColumnName("registration_id");
            e.Property(r => r.UserId).HasColumnName("user_id");
            e.Property(r => r.TalkshowId).HasColumnName("talkshow_id");
            e.Property(r => r.RegisteredAt).HasColumnName("registered_at");
            e.Property(r => r.SeatCode).HasColumnName("seat_code").HasMaxLength(20);
            e.HasIndex(r => new { r.UserId, r.TalkshowId }).IsUnique();
            e.HasOne(r => r.User).WithMany(u => u.TalkshowRegistrations)
                .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Talkshow).WithMany(t => t.Registrations)
                .HasForeignKey(r => r.TalkshowId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.ToTable("audit_logs");
            e.HasKey(l => l.LogId);
            e.Property(l => l.LogId).HasColumnName("log_id");
            e.Property(l => l.UserId).HasColumnName("user_id");
            e.Property(l => l.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
            e.Property(l => l.EntityType).HasColumnName("entity_type").HasMaxLength(100);
            e.Property(l => l.EntityId).HasColumnName("entity_id").HasMaxLength(255);
            e.Property(l => l.OldValue).HasColumnName("old_value");
            e.Property(l => l.NewValue).HasColumnName("new_value");
            e.Property(l => l.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
            e.Property(l => l.OccurredAt).HasColumnName("occurred_at");
        });
    }
}
