using Architecture.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Architecture.Database
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users", "User");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Status).IsRequired();

            builder.OwnsOne(x => x.FullName, y =>
            {
                y.Property(x => x.Name).HasColumnName(nameof(UserEntity.FullName.Name)).IsRequired().HasMaxLength(100);
                y.Property(x => x.Surname).HasColumnName(nameof(UserEntity.FullName.Surname)).IsRequired().HasMaxLength(200);
            });

            builder.OwnsOne(x => x.Email, y =>
            {
                y.Property(x => x.Address).HasColumnName(nameof(UserEntity.Email)).IsRequired().HasMaxLength(300);
                y.HasIndex(x => x.Address).IsUnique();
            });

            builder.OwnsOne(x => x.Auth, y =>
            {
                y.Property(x => x.Login).HasColumnName(nameof(UserEntity.Auth.Login)).IsRequired().HasMaxLength(100);
                y.Property(x => x.Password).HasColumnName(nameof(UserEntity.Auth.Password)).IsRequired().HasMaxLength(500);
                y.Property(x => x.Salt).HasColumnName(nameof(UserEntity.Auth.Salt)).IsRequired().HasMaxLength(500);
                y.Property(x => x.Roles).HasColumnName(nameof(UserEntity.Auth.Roles)).IsRequired();
                y.HasIndex(x => x.Login).IsUnique();
            });

            builder.HasMany(x => x.UsersLogs).WithOne(x => x.User).HasForeignKey(x => x.UserId);
        }
    }
}
