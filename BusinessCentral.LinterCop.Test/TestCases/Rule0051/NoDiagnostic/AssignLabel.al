codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        myLabel: Label 'MyLabel';
        myText: Text[3];
    begin
        myText := [|myLabel|];
    end;
}