codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    begin
        [|RecordReturnValue().Get('10000')|];
    end;

    procedure RecordReturnValue() ItemVariant: Record "Item Variant"
    begin
    end;
}

table 50100 "Item Variant"
{
    fields
    {
        field(1; "Code"; Code[10]) { }
        field(2; "Item No."; Code[20]) { }
    }
    keys
    {
        key(Key1; "Item No.", "Code") { }
    }
}