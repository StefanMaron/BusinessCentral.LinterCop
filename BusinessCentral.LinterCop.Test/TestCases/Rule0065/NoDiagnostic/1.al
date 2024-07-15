codeunit 50000 MyCodeunit
{
    [IntegrationEvent(false, false)]
    local procedure MyProcedure(var MyTable: Record MyTable; mytable2: Record MyTable)
    begin
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::MyCodeunit, MyProcedure, '', false, false)]
    local procedure MyProcedure2(var [|MyTable|]: Record MyTable; MyTable2: Record MyTable)
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