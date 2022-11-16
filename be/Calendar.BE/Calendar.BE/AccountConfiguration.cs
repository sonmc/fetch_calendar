using Calendar.BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.BE
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasData
            (
                new Account
                {
                    Id = 1,
                    Name = "SonMc",
                    Email = "sonmc90@gmail.com"
                },
                new Account
                {
                    Id=2,
                    Name = "SonMc222",
                    Email = "maicongson@zen8labs.com"
                }
            );
            
        }
    }
}
