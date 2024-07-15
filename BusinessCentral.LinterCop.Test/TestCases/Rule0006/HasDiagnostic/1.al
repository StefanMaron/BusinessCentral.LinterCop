table 50100 MyTable
{
    TableType = Temporary;
    DataClassification = ToBeClassified;

    fields
    {
        field(1; MyField; Integer)
        {
            [|AutoIncrement = true;|]
            DataClassification = ToBeClassified;
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