using MetadataStorage.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace MetadataStorage.Infrastructure.Database
{
    public class MetadataDbContext : DbContext
    {
        public MetadataDbContext(DbContextOptions<MetadataDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        /// <summary>
        /// Gets or sets the MetadataFields
        /// </summary>
        public DbSet<MetadataField> MetadataFields { get; set; }

        /// <summary>
        /// Gets or sets the MetadataEntity
        /// </summary>
        public DbSet<EntityMetadataDetails> EntityMetadataDetails { get; set; }

        /// <summary>
        /// Gets or sets the Metadata schema details
        /// </summary>
        public DbSet<MetadataSchemaDetails> MetadataSchemaDetails { get; set; }

        /// <summary>
        /// Gets or sets the Metadata schema field mapping
        /// </summary>
        public DbSet<MetadataSchemaFields> MetadataSchemaFields { get; set; }

        /// <summary>
        /// Gets or sets the Tenant Metadata schema 
        /// </summary>
        public DbSet<EntityMetadataSchema> EntityMetadataSchemas { get; set; }

        /// <summary>
        /// Gets or sets the Tenant metadata metadata values
        /// </summary>
        public DbSet<EntityMetadataFieldValues> EntityMetadataFieldValues { get; set; }

    }
}