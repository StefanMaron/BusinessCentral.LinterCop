codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    begin
        [|MYPROCEDURE|]();
    end;

    procedure "My Procedure"()
    begin
        [|"MY PROCEDURE"|]();
    end;
}