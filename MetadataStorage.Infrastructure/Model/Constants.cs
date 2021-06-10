namespace MetadataStorage.Infrastructure.Model
{
    public static class Constants
    {
        public static class ErrorMessage
        {
            /// <summary>
            /// The access denied
            /// </summary>
            public const string AccessDenied = "Access Denied !!";

            /// <summary>
            /// Invalid parameter
            /// </summary>
            public const string InvalidParameter = "Invalid Paramater!";

            /// <summary>
            /// Field value type mismatch
            /// </summary>
            public const string FieldValueTypeMismatch = "The field value does not match type";

            /// <summary>
            /// Public fields not available
            /// </summary>
            public const string PublicFieldNotAvailable = "Public fields are not available, validation failed!!";

            /// <summary>
            /// Transform result empty
            /// </summary>
            public const string TransformResultEmpty = "Empty metadata transform result, validation failed!!";

            /// <summary>
            /// Transformation not successful
            /// </summary>
            public const string TransformNotSucsessfull = "Transformation operation was not successful, validation failed!!";

            /// <summary>
            /// Missing mandatory field
            /// </summary>
            public const string MissingMandatoryField = "Missing mandatory fields";

            /// <summary>
            /// Metadata schema or fields are not present
            /// </summary>
            public const string MetadataFieldsNotFound = "Metadata fields or schema not found!!";

            /// <summary>
            /// No backing metadatafield
            /// </summary>
            public const string NoMetadataField = "Metadatafield not found for public field";

            /// <summary>
            /// Metadata field value not present
            /// </summary>
            public const string MetadataFieldValueNotAvailable = "Value for the metadata field not found!";

            /// <summary>
            /// Extra stored field value
            /// </summary>
            public const string MetadataFieldWithNoSchema = "A metadata value is found with no metadata field in schema";

            /// <summary>
            /// Metadata fields not available
            /// </summary>
            public const string MetadataFieldNotAvailable = "Metadata fields are not available, validation failed!!";

            /// <summary>
            /// Metadata fields not available
            /// </summary>
            public const string MetadataFieldMandatory = "Metadata field is mandatory, delete operation failed!!";


            /// <summary>
            /// The record updated
            /// </summary>
            public const string RecordUpdated = "Record updated successfully.";

            /// <summary>
            /// The new record created
            /// </summary>
            public const string NewRecordCreated = "New record created successfully.";

            /// <summary>
            /// The record deleted
            /// </summary>
            public const string RecordDeleted = "Record deleted successfully.";

            /// <summary>
            /// The duplicate tenant
            /// </summary>
            public const string DuplicateMetadataFieldTenant = "Environment already has metadata field with same field name and public name.";

            /// <summary>
            /// Schema field tenant error
            /// </summary>
            public const string SchemaFieldTenantError = "Metadata schema and Metadata field do not belong to same tenant";

            /// <summary>
            /// The duplicate tenant
            /// </summary>
            public const string DuplicateMetadataSchemaTenant = "Environment already has metadata field with same field name and public name.";

            /// <summary>
            /// Metadata schema or fields are not present
            /// </summary>
            public const string MetadataSchemaNotFound = "Metadata schema not found!!";

            /// <summary>
            /// The duplicate tenant
            /// </summary>
            public const string DuplicateMetadataFieldValue = "Entity metadata field with value already exists.";

            // <summary>
            /// The duplicate tenant
            /// </summary>
            public const string DuplicateMetadataEntityTenant = "Environment already has metadata entity with same name.";

            /// <summary>
            /// The user not found
            /// </summary>
            public const string MetadataEntityNotFound = "Metadata entity could not be found or you do not have access.";

            /// <summary>
            /// The user not found
            /// </summary>
            public const string UserNotFound = "User data could not be found.";
        }
    }
}