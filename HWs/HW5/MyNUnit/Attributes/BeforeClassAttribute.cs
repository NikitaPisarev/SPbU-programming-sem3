namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class BeforeClass : Attribute
{
    public BeforeClass()
    {
    }
}