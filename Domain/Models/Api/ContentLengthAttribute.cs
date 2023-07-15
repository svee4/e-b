using System.ComponentModel.DataAnnotations;

namespace e_b.Domain.Models.Api;

public class ContentLengthAttribute : ValidationAttribute
{
	private readonly int _maxSize;
	private readonly int _minSize;

	public ContentLengthAttribute(int minSize, int maxSize) =>
		(_minSize, _maxSize) = (minSize, maxSize);


	public ContentLengthAttribute(int maxSize) : this(0, maxSize) { }

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is null)
			return ValidationResult.Success;

		if (value is not IFormFile file)
			throw new ArgumentException("Value must be an IFormFile");

		if (file.Length > _maxSize)
		{
			switch (_maxSize)
			{
				case < 1024:
					return new ValidationResult($"File too big (max: {_maxSize}B)");
				case < 1024 * 1024:
					return new ValidationResult($"File too big (max: {_maxSize / 1024}KB)");
				default:
					return new ValidationResult($"File too big (max: {_maxSize / 1024 / 1024}MB)"); //
			}
		}

		if (file.Length < _minSize)
		{
			switch (_minSize)
			{
				case < 1024:
					return new ValidationResult($"File too small (min: {_minSize}B)");
				case < 1024 * 1024:
					return new ValidationResult($"File too small (min: {_minSize / 1024}KB)");
				default:
					return new ValidationResult($"File too small (min: {_minSize / 1024 / 1024}MB)");
			}
		}

		return ValidationResult.Success;
	}
}