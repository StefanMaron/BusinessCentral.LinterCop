codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        ItemVariant: Record "Item Variant";
    begin
        [|ItemVariant.Get(ItemVariant."Record ID")|];
    end;
}

table 50100 "Item Variant"
{
    fields
    {
        field(1; "Code"; Code[10]) { }
        field(2; "Item No."; Code[20]) { }
        field(3; "Record ID"; RecordId) { }
    }
    keys
    {
        key(Key1; "Item No.", "Code") { }
    }
}