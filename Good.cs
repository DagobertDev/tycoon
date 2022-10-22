namespace Tycoon;

public record Good(string Name)
{
	public override string ToString()
	{
		return Name;
	}
}
