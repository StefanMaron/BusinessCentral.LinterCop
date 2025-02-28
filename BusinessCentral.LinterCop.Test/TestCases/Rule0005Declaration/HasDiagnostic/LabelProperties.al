table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer)
        {
            Caption = 'My Caption', [|COMMENT|] = 'My Comment', [|LOCKED|] = false, [|MAXLENGTH|] = 100;
        }
    }
}

codeunit 50100 MyCodeunit
{
    var
        MyLabelLbl: Label 'My Label', [|COMMENT|] = 'My Comment', [|LOCKED|] = false, [|MAXLENGTH|] = 100;
}