using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AngularNetBase.Practice.Infrastructure.Extensions
{
    public static class XminConcurrencyExtensions
    {
        public static EntityTypeBuilder UseXminAsConcurrencyToken(this EntityTypeBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Property<uint>("xmin")
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();

            return builder;
        }
    }
}
