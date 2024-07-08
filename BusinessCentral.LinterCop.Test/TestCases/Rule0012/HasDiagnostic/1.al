codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    begin
        [|Codeunit.Run(50100);|]
    end;
}