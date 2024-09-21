// testcase for a normal assignment with semicolon (not an empty statement)
codeunit 50100 MyCodeunit
{
    procedure MyProcedure(Param: Boolean)
    var
        LocalVar: Boolean;
    begin
        LocalVar := Param[|;|]
    end;
}