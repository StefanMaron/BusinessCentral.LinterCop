codeunit 50000 MyCodeunit
{
    [IntegrationEvent(false, false)]
    local procedure MyProcedure(var MyTable: Record MyTable; var MyTable3: Record MyTable; var MyTable2: Record MyTable)
    begin
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::MyCodeunit, MyProcedure, '', false, false)]
    local procedure MyProcedure2(var MyTable: Record MyTable; var Mytable2: Record MyTable; [|MyTable3|]: Record MyTable)
    begin

    end;
}

table 50000 MyTable
{
    Caption = '', Locked = true;

    fields
    {
        field(1; MyField; Integer)
        {
            Caption = '', Locked = true;
            DataClassification = ToBeClassified;
        }
    }
}