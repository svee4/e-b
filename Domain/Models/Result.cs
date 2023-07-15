namespace e_b.Domain.Models;


public struct Result<T, E>
{
	public T? Value { get; private set; }
	public E? Error { get; private set; }

	public bool IsOk => Value is not null;
	public bool IsErr => Error is not null;

	public static Result<T, E> Ok(T value) => new() { Value = value };
	public static Result<T, E> Err(E error) => new() { Error = error };

	public static implicit operator Result<T, E>(T value) => Ok(value);
	public static implicit operator Result<T, E>(E error) => Err(error);


	public override string ToString()
	{
		if (IsOk)
		{
			return $"Ok({Value})";
		}
		else
		{
			return $"Err({Error})";
		}
	}

}