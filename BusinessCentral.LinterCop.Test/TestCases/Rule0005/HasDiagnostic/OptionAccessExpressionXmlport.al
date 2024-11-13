codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    begin
        Xmlport.Run([|XmlPort|]::MyXmlport);
    end;
}

xmlport 50100 MyXmlport { }