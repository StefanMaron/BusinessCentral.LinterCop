table 50100 MyTable
{
    fields
    {
        field(1; Name; Text[100]) { }
    }

    procedure DoSth()
    begin
    end;
}

codeunit 50000 MyCodeunit
{
    [IntegrationEvent(false, false)]
    local procedure MyProcedure(var Rec: Record MyTable; var xRec: Record MyTable)
    begin
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::MyCodeunit, MyProcedure, '', false, false)]
    local procedure MyProcedure2(var Rec: Record MyTable; xRec: Record MyTable)
    begin
        [|Rec|].DoSth();
    end;
}