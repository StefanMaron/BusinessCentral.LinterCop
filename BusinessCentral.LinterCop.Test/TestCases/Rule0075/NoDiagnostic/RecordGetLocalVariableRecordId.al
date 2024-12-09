codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        ItemVariant: Record "Item Variant";
        MyRecordId: RecordId;
    begin
        [|ItemVariant.Get(MyRecordId)|];
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