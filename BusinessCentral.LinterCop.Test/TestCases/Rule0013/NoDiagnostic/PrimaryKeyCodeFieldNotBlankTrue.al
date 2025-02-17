table 50100 MyTable
{
    fields
    {
        [|field(1; MyField; Code[10])|]
        {
            NotBlank = true;
        }
    }

    keys
    {
        key(Key1; MyField) { }
    }
}