codeunit 50100 MyCodeunit
{
    /// <summary>
    /// Duplicate documentation parameter.
    /// </summary>
    /// <param name="Value">The parameter documentation.</param>
    /// [|<param name="Value">The duplicate parameter documentation.</param>|]
    procedure MyProcedure(Value: Boolean)
    begin

    end;
}