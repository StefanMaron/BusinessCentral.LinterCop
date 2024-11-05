codeunit 50100 MyCodeunit
{
    /// <summary>
    /// Documentation comment with returns but no return value.
    /// </summary>
    /// [|<returns>Some value.</returns>|]
    procedure DoesNotReturn()
    begin

    end;

    /// <summary>
    /// Return value but no documentation comment returns.
    /// </summary>
    procedure DoesReturnButNoComment() [|ReturnValue: Boolean|]
    begin

    end;
}