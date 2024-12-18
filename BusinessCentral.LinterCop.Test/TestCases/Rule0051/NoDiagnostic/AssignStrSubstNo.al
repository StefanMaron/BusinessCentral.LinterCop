codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myText: Text[3];
    begin
        myText := [|StrSubstNo('%1', 123)|];
    end;
}