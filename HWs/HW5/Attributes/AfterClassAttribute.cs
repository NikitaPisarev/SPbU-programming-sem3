namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AfterClass : Attribute
{
    public AfterClass()
    {
    }
}