namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class Before : Attribute
{
    public Before()
    {
    }
}