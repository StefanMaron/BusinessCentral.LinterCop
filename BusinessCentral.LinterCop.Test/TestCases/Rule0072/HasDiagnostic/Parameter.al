codeunit 50100 MyCodeunit
{
    /// <summary>
    /// Documentation comment parameter but no procedure parameter.
    /// </summary>
    /// [|<param name="Value">The value.</param>|]
    procedure NoParameter()
    begin

    end;

    /// <summary>
    /// Procedure parameter but no documentation comment parameter.
    /// </summary>
    procedure ParameterButNoComment([|Value: Boolean|])
    begin

    end;

    /// <summary>
    /// Parameter name mismatch.
    /// </summary>
    /// [|<param name="NotMyValue">The value.</param>|]
    procedure NameMissmatch([|Value: Boolean|])
    begin

    end;
}