table 50100 MyTable
{
    DataClassification = ToBeClassified;

    fields
    {
        [|field(1; MyField; Code[10])|]
        {
            NotBlank = true;
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