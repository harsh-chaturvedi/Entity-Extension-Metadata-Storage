using System.Collections.Generic;

namespace MetadataStorage.Infrastructure.Model
{
    public class SearchInputDTO : SearchInputContext
    {
        public string SearchInput { get; set; }

        public string SearchMetadataEntityName { get; set; }

        public List<string> MetadataSearchFields { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public bool SearchMetadata { get; set; }
    }
}