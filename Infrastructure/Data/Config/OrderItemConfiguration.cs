using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            //There will be no primary key, To avoid error from EF
            //builder.HasNoKey();
            //We have an Order owned property in here, This is the item ordered
            builder.OwnsOne(i => i.ItemOrdered, io => { io.WithOwner(); });
            //price configuration
            builder.Property(i => i.Price).HasColumnType("decimal(18,2)");
        }
    }
}
