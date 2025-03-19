table 1 MyTable
{
    fields
    {
        field(1; Field1; Integer)
        {
            Caption = '', Locked = true;
        }
    }
}
query 1 QueryCaption
{
    [|Caption|] = 'QueryCaption_Caption';

    elements
    {
        dataitem(MyTable; MyTable)
        {
            column(MyField1; Field1)
            {
                Caption = 'Field1_Caption';
            }
        }
    }
}