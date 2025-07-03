using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Helpers;

public static class Rule0083BuiltInDateTimeMethodHelper
{
    public static InvocationExpressionSyntax? GetReplacementMethod(string methodName, InvocationExpressionSyntax invocationExpression)
    {
        var arguments = invocationExpression.ArgumentList.Arguments;

        return methodName switch
        {
            "Date2DMY" when arguments.Count == 2 => GetDate2DMYReplacement(arguments[1]),
            "Date2DWY" when arguments.Count == 2 => GetDate2DWYReplacement(arguments[1]),
            "DT2Date" => GetInvocationExpressionSyntax("Date"),
            "DT2Time" => GetInvocationExpressionSyntax("Time"),
            "Format" when arguments.Count == 3 => GetFormatReplacement(arguments[2]),
            _ => null
        };
    }

    private static InvocationExpressionSyntax? GetDate2DMYReplacement(CodeExpressionSyntax argument)
    {
        return GetIntegerValueOrNull(argument) switch
        {
            1 => GetInvocationExpressionSyntax("Day"),
            2 => GetInvocationExpressionSyntax("Month"),
            3 => GetInvocationExpressionSyntax("Year"),
            _ => null
        };
    }

    private static InvocationExpressionSyntax? GetDate2DWYReplacement(CodeExpressionSyntax argument)
    {
        return GetIntegerValueOrNull(argument) switch
        {
            1 => GetInvocationExpressionSyntax("DayOfWeek"),
            2 => GetInvocationExpressionSyntax("WeekNo"),
            3 => null, // Year() isn't returning the same value. When the input date to the Date2DWY method is in a week that spans two years, the Date2DWY method computes the output year as the year that has the most days.
            _ => null
        };
    }

    private static InvocationExpressionSyntax? GetFormatReplacement(CodeExpressionSyntax argument)
    {
        return argument.GetIdentifierOrLiteralValue() switch
        {
            "<HOURS24>" => GetInvocationExpressionSyntax("Hour"),
            "<MINUTES>" => GetInvocationExpressionSyntax("Minute"),
            "<SECONDS>" => GetInvocationExpressionSyntax("Second"),
            "<THOUSANDS>" => GetInvocationExpressionSyntax("Millisecond"),
            _ => null
        };
    }

    private static int? GetIntegerValueOrNull(CodeExpressionSyntax argument)
        => int.TryParse(argument.GetIdentifierOrLiteralValue(), out var value) ? value : null;

    private static InvocationExpressionSyntax GetInvocationExpressionSyntax(string identifier)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.IdentifierName(identifier),
            SyntaxFactory.ArgumentList());
    }
}