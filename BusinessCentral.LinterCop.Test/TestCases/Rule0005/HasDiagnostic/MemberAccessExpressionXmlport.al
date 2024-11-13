codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|XmlPort|]::MyXmlport;
    end;
}

xmlport 50100 MyXmlport { }