table 50108 MyTable
{
    fields
    {
        field(1; MyField; Code[20]) { }

        field(3; [|MySecondField|]; Code[10])
        {
            TableRelation = MyTable;
        }
    }

    keys
    {
        key(PK; MyField) { }
    }
}