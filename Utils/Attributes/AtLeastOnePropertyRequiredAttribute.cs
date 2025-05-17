using System.ComponentModel.DataAnnotations;

namespace Seiun.Utils.Attributes;

/// <summary>
/// 至少提供一个属性
/// </summary>
/// <param name="propertyNames">需要至少提供一个的可选属性 空为全部</param>
/// <param name="errorMessage">错误时的信息</param>
public class AtLeastOnePropertyRequiredAttribute(string[]? propertyNames = null, string? errorMessage = null)
	: ValidationAttribute(errorMessage ?? "At least one property is required")
{
	override protected ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		string thisErrorMessage;

		// 如果没有指定属性，则判断所有属性
		if (propertyNames == null || propertyNames.Length == 0)
		{
			var properties = validationContext.ObjectType.GetProperties();
			if (properties.Any(property => property.GetValue(validationContext.ObjectInstance) != null))
				return ValidationResult.Success;

			thisErrorMessage = ErrorMessage ?? $"At least one property is required: {
				string.Join(", ", properties.Select(property => property.Name))
			}";
			return new ValidationResult(thisErrorMessage);
		}

		// 判断指定属性
		foreach (var propertyName in propertyNames)
		{
			var property = validationContext.ObjectType.GetProperty(propertyName);
			if (property == null) continue;

			var propertyValue = property.GetValue(validationContext.ObjectInstance);
			if (propertyValue is string strVal && !string.IsNullOrWhiteSpace(strVal)) return ValidationResult.Success;
		}

		thisErrorMessage = ErrorMessage ?? $"At least one property is required: {string.Join(", ", propertyNames)}";
		return new ValidationResult(thisErrorMessage);
	}
}
