table 50100 MyTable
{
    DataClassification = ToBeClassified;

    fields
    {
        [|field(1; MyField; Code[10])|]
        {
            DataClassification = ToBeClassified;
        }
        field(2; MyField2; Integer)
        {
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(Key1; MyField, MyField2)
        {
            Clustered = true;
        }
    }
}