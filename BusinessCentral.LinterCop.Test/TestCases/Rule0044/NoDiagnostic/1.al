tableextension 50100 Customer extends Customer
{
    fields
    {
        [|field(50100; MyField; Integer) { }|]
    }
}

tableextension 50101 Contact extends Contact
{
    fields
    {
        field(50100; MyField; Integer) { }
    }
}

table 18 Customer
{
    fields
    {
        field(1; "No."; Code[20]) { }
    }
}
table 5050 Contact
{
    fields
    {
        field(1; "No."; Code[20]) { }
    }
}
