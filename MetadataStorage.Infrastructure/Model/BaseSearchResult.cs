using System.Collections.Generic;

namespace MetadataStorage.Infrastructure.Model
{
    public class SearchResult<T> where T : BaseSearchResult
    {
        public long TotalCount { get; set; }

        public int CurrentCount { get; set; }

        public string CurrentValueRange { get; set; }

        public List<T> Entities { get; set; }

        public bool IsError { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class BaseSearchResult
    {
        public List<PublicFieldValue> MetadataValue { get; set; }
    }
}