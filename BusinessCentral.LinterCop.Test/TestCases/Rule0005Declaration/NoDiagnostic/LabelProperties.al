table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer)
        {
            Caption = 'My Caption', [|Comment|] = 'My Comment', [|Locked|] = false, [|MaxLength|] = 100;
        }
    }
}

codeunit 50100 MyCodeunit
{
    var
        MyLabelLbl: Label 'My Label', [|Comment|] = 'My Comment', [|Locked|] = false, [|MaxLength|] = 100;
}