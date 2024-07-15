codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    begin
        [|Codeunit.Run(Codeunit::MyCodeunit);|]
    end;
}