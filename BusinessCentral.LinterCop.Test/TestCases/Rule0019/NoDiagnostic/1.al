table 50100 MyTable
{
    DataClassification = CustomerContent;

    fields
    {
        [|field(1; MyField; Integer)|]
        {
            DataClassification = SystemMetadata;
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