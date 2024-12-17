codeunit 50100 MyCodeunit
{
    procedure MyProcedure(ItemNo: Code[20]; VariantCode: Code[20])
    var
        ItemVariant: Record "Item Variant";
    begin
        [|ItemVariant.Get(ItemNo, VariantCode)|];
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