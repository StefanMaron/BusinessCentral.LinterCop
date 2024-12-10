xmlport 50100 MyXmlport
{
    schema
    {
        tableelement(ItemVariant; "Item Variant") { }
    }

    trigger OnPreXmlPort()
    begin
        [|ItemVariant.Get('10000', 'VARIANTCODE')|];
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