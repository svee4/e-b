namespace e_b.Domain.Models;

public struct Option<T>
{
	private T? _value;

	private Option(T? value) =>
		_value = value;


	public static Option<T> Some(T value) => new(value);
	public static Option<T> None() => new();

	public static implicit operator Option<T>(T? value) => value is null ? None() : Some(value);



	public Option<TResult> Map<TResult>(Func<T, TResult> map) =>
		_value is not null ? map(_value) : Option<TResult>.None();

	public T Reduce(T defaultValue) =>
		_value is not null ? _value : defaultValue;

	public T Reduce(Func<T> defaultFactory) =>
		_value is not null ? _value : defaultFactory();

	public T Unwrap() =>
		_value ?? throw new InvalidOperationException($"Cannnot unwrap None value from Option<{typeof(T)}>");


	public bool HasValue => _value is not null;

	public override string ToString() =>
		Map(v => $"Some({v})").Reduce("None");

}