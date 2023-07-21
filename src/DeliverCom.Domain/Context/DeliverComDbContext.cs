using DeliverCom.Domain.Delivery.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace DeliverCom.Domain.Context
{
    public class DeliverComDbContext : DbContext
    {
        public DeliverComDbContext()
        {
        }

        public DeliverComDbContext(DbContextOptions<DeliverComDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Company.Company> Companies { get; set; }
        public virtual DbSet<User.User> Users { get; set; }
        public virtual DbSet<Delivery.Delivery> Deliveries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User.User>(builder =>
            {
                builder.HasKey("Id");
                builder.Property(user => user.EntityId).HasColumnName(nameof(User.User.EntityId));
                builder.Property(user => user.CreateDate).HasColumnName(nameof(User.User.CreateDate));
                builder.Property(user => user.UpdateDate).HasColumnName(nameof(User.User.UpdateDate));
                builder.Property(user => user.IsActive).HasColumnName(nameof(User.User.IsActive));
                builder.Property(user => user.Email).HasColumnName(nameof(User.User.Email));
                builder.Property(user => user.FirstName).HasColumnName(nameof(User.User.FirstName));
                builder.Property(user => user.Surname).HasColumnName(nameof(User.User.Surname));
                builder.Property(user => user.Password).HasColumnName(nameof(User.User.Password));
                builder.Property(user => user.Role).HasColumnName(nameof(User.User.Role));
                builder.HasOne(user => user.Company).WithMany(entity => entity.Users).HasForeignKey("CompanyId");
                builder.ToTable("Users");
            });

            modelBuilder.Entity<Company.Company>(builder =>
            {
                builder.HasKey("Id");
                builder.Property(company => company.EntityId).HasColumnName(nameof(Company.Company.EntityId));
                builder.Property(company => company.CreateDate).HasColumnName(nameof(Company.Company.CreateDate));
                builder.Property(company => company.UpdateDate).HasColumnName(nameof(Company.Company.UpdateDate));
                builder.Property(company => company.IsActive).HasColumnName(nameof(Company.Company.IsActive));
                builder.Property(company => company.Name).HasColumnName(nameof(Company.Company.Name));
                builder.ToTable("Companies");
            });

            modelBuilder.Entity<Delivery.Delivery>(builder =>
            {
                builder.HasKey("Id");
                builder.Property(delivery => delivery.EntityId).HasColumnName(nameof(Delivery.Delivery.EntityId));
                builder.Property(delivery => delivery.CreateDate).HasColumnName(nameof(Delivery.Delivery.CreateDate));
                builder.Property(delivery => delivery.UpdateDate).HasColumnName(nameof(Delivery.Delivery.UpdateDate));
                builder.Property(delivery => delivery.IsActive).HasColumnName(nameof(Delivery.Delivery.IsActive));
                builder.Property(delivery => delivery.DeliveryStatus).HasColumnName(nameof(Delivery.Delivery.DeliveryStatus));
                builder.Property(delivery => delivery.DeliveryNumber).HasColumnName(nameof(Delivery.Delivery.DeliveryNumber));
                builder.OwnsOne(delivery => delivery.SenderAddress, navigationBuilder =>
                {
                    navigationBuilder.Property(address => address.City)
                        .HasColumnName($"{nameof(Delivery.Delivery.SenderAddress)}{nameof(Address.City)}");
                    navigationBuilder.Property(address => address.Street)
                        .HasColumnName($"{nameof(Delivery.Delivery.SenderAddress)}{nameof(Address.Street)}");
                    navigationBuilder.Property(address => address.Town)
                        .HasColumnName($"{nameof(Delivery.Delivery.SenderAddress)}{nameof(Address.Town)}");
                    navigationBuilder.Property(address => address.ZipCode)
                        .HasColumnName($"{nameof(Delivery.Delivery.SenderAddress)}{nameof(Address.ZipCode)}");
                });

                builder.OwnsOne(delivery => delivery.DeliveryAddress, navigationBuilder =>
                {
                    navigationBuilder.Property(address => address.City)
                        .HasColumnName($"{nameof(Delivery.Delivery.DeliveryAddress)}{nameof(Address.City)}");
                    navigationBuilder.Property(address => address.Street)
                        .HasColumnName($"{nameof(Delivery.Delivery.DeliveryAddress)}{nameof(Address.Street)}");
                    navigationBuilder.Property(address => address.Town)
                        .HasColumnName($"{nameof(Delivery.Delivery.DeliveryAddress)}{nameof(Address.Town)}");
                    navigationBuilder.Property(address => address.ZipCode)
                        .HasColumnName($"{nameof(Delivery.Delivery.DeliveryAddress)}{nameof(Address.ZipCode)}");
                });
                builder.HasOne(user => user.Company).WithMany(entity => entity.Deliveries).HasForeignKey("CompanyId");
                builder.ToTable("Deliveries");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}