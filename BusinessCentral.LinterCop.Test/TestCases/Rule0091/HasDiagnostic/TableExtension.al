tableextension 1 TableExt extends Table
{
    [|Caption|] = 'TableExt_Caption';

    fields
    {
        field(2; MyField2; Integer)
        {
            [|Caption|] = 'TableFieldCaption_MyField2_Caption';
            [|ToolTip|] = 'TableFieldToolTip_MyField2_ToolTip';
        }
    }
}

table 1 Table
{
    fields
    {
        field(1; MyField; Integer) { Caption = '', Locked = true; }
    }
}