table 50100 MyTable
{
    fields
    {
        [|field(1; MyField; Code[10])|] { }
        field(2; MyField2; Integer) { }
    }

    keys
    {
        key(Key1; MyField, MyField2) { }
    }
}