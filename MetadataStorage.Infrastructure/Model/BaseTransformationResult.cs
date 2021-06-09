using System;
using System.Collections.Generic;

namespace MetadataStorage.Infrastructure.Model
{
    public class BaseTransformationResult
    {
        public Guid EntityId { get; set; }

        public OperationResult OperationResult { get; set; }

        public string ErrorMessage { get; set; }

        public List<string> Errors { get; set; }

        public List<MetadataFieldError> MetadataFieldErrors { get; set; }
    }
}