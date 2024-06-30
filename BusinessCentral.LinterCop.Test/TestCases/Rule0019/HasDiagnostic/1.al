table 50100 MyTable
{
    DataClassification = CustomerContent;

    fields
    {
        field(1; MyField; Integer)
        {
            [|DataClassification = CustomerContent;|]
        }
    }

    keys
    {
        key(Key1; MyField)
        {
            Clustered = true;
        }
    }
}