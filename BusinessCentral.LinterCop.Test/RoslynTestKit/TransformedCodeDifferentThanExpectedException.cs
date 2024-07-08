namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public class TransformedCodeDifferentThanExpectedException : RoslynTestKitException
{
    public string Diff { get; }
    public string TransformedCode { get; }
    public string ExpectedCode { get; }

    public TransformedCodeDifferentThanExpectedException(string transformedCode, string expectedCode, string diff)
        : base($"Transformed code is different than expected:{Environment.NewLine}{diff}")
    {
        Diff = diff;
        TransformedCode = transformedCode;
        ExpectedCode = expectedCode;
    }
}