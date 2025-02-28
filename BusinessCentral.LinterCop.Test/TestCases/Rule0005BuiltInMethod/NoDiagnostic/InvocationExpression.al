codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    begin
        [|MyProcedure|]();
    end;

    procedure "My Procedure"()
    begin
        [|"My Procedure"|]();
    end;
}