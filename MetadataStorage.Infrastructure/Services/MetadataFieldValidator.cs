using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MetadataStorage.Infrastructure.Contracts;
using MetadataStorage.Infrastructure.Model;
using Newtonsoft.Json.Linq;

namespace MetadataStorage.Infrastructure.Services
{
    public class MetadataFieldValidator : IMetadataValidator
    {
        private readonly IMetaDataStorageService _metaDataService;
        public MetadataFieldValidator(IMetaDataStorageService metaDataService)
        {
            _metaDataService = metaDataService;
        }

        public (bool, string) ValidateMetadataFieldValuesForSchema(MetadataFieldTransformationResult transformationResult)
        {
            var (isSuccess, errorMessage) = PerformBasicValidation(transformationResult);

            if (!isSuccess)
                return (isSuccess, errorMessage);

            if (transformationResult.MetadataFieldValues == null || !transformationResult.MetadataFieldValues.Any())
            {
                return (false, Constants.ErrorMessage.MetadataFieldNotAvailable);
            }
            return (true, string.Empty);
        }

        public (bool, string) ValidatePublicFieldValuesForType(MetadataFieldTransformationResult transformationResult)
        {
            var (isSuccess, errorMessage) = PerformBasicValidation(transformationResult);

            if (!isSuccess)
                return (isSuccess, errorMessage);

            var entityMetadataFields = _metaDataService.GetMetadataFieldsByEntityId(transformationResult.EntityId);

            List<MetadataFieldError> validationError = new List<MetadataFieldError>();

            foreach (var item in transformationResult.MetadataFieldValues)
            {
                var metadata = entityMetadataFields.FirstOrDefault(x => x.Id.Equals(item.FieldId));
                MetadataFieldError error;

                switch (metadata.FieldType)
                {
                    case FieldType.BOOL:
                        error = !bool.TryParse(item.FieldValue, out bool boolval)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.FieldValueTypeMismatch}: {metadata.FieldType}"
                            } : null;
                        break;
                    case FieldType.INT:
                        error = !int.TryParse(item.FieldValue, out int intval)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.FieldValueTypeMismatch}: {metadata.FieldType}"
                            } : null;
                        break;
                    case FieldType.LONG:
                        error = !long.TryParse(item.FieldValue, out long longval)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.FieldValueTypeMismatch}: {metadata.FieldType}"
                            } : null;
                        break;
                    case FieldType.FLOAT:
                        error = !float.TryParse(item.FieldValue, out float floatval)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.FieldValueTypeMismatch} : {metadata.FieldType}"
                            } : null;
                        break;
                    case FieldType.DOUBLE:
                        error = !double.TryParse(item.FieldValue, out double doubleval)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.FieldValueTypeMismatch} : {metadata.FieldType}"
                            } : null;
                        break;
                    case FieldType.STRING:
                        error = metadata.IsMandatory && string.IsNullOrEmpty(item.FieldValue)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.MissingMandatoryField} : {metadata.FieldType}"
                            } : null;
                        break;
                    case FieldType.JSON:
                        error = !IsValidJSON(item.FieldValue)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.FieldValueTypeMismatch} : {metadata.FieldType}"
                            } : null;
                        break;
                    case FieldType.EMAIL:
                        error = !IsValidEmail(item.FieldValue)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.FieldValueTypeMismatch} : {metadata.FieldType}"
                            } : null;
                        break;
                    case FieldType.DATETIME:
                        error = !DateTime.TryParse(item.FieldValue, out DateTime dateTimeResult)
                            ? new MetadataFieldError
                            {
                                MetadataField = metadata,
                                ErrorPhrase = $"{Constants.ErrorMessage.FieldValueTypeMismatch} : {metadata.FieldType}"
                            } : null;
                        break;
                    default:
                        error = null;
                        break;
                }

                if (error != null)
                {
                    validationError.Add(error);
                    error = null;
                }
            }

            if (validationError != null && validationError.Any())
            {
                return (false, $"{Constants.ErrorMessage.MissingMandatoryField}: {string.Join(", ", validationError.Select(x => x.MetadataField.FieldName))}");
            }

            return (true, string.Empty);
        }

        public (bool, string) ValidateDatabaseFieldValuesForSchema(PublicFieldTransformationResult transformationResult)
        {
            var (isSuccess, errorMessage) = PerformBasicValidation(transformationResult);

            if (!isSuccess)
                return (isSuccess, errorMessage);

            if (transformationResult.PublicFieldValues == null || !transformationResult.PublicFieldValues.Any())
            {
                return (false, Constants.ErrorMessage.PublicFieldNotAvailable);
            }
            return (true, string.Empty);
        }

        private (bool, string) PerformBasicValidation(BaseTransformationResult transformationResult)
        {
            if (transformationResult == null)
            {
                return (false, Constants.ErrorMessage.TransformResultEmpty);
            }

            if (transformationResult.OperationResult != OperationResult.Success)
            {
                if (!string.IsNullOrEmpty(transformationResult.ErrorMessage))
                {
                    return (false, transformationResult.ErrorMessage);
                }
                if (transformationResult.MetadataFieldErrors == null && transformationResult.Errors == null)
                {
                    return (false, Constants.ErrorMessage.TransformNotSucsessfull);
                }
            }

            //error for missing backing field - extra fields (metadata field not mapped to schema)
            if (transformationResult.Errors != null && transformationResult.Errors.Any())
            {
                return (false, string.Join(", ", transformationResult.Errors));
            }

            //error for missing mandatory field in input (public fields)
            if (transformationResult.MetadataFieldErrors != null && transformationResult.MetadataFieldErrors.Any())
            {
                return (false, $"{Constants.ErrorMessage.MissingMandatoryField}: {string.Join(", ", transformationResult.MetadataFieldErrors.Select(x => x.MetadataField.FieldName))}");
            }

            return (true, string.Empty);
        }

        private static bool IsValidEmail(string email)
        {
            var emailAddressAttribute = new EmailAddressAttribute();
            return emailAddressAttribute.IsValid(email);
        }

        public bool IsValidJSON(string json)
        {
            try
            {
                JToken token = JObject.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}