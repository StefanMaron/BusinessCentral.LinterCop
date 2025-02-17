table 50108 "My Table"
{
    fields
    {
        field(1; MyField; Code[20]) { }

        field(2; [|"My TableRelation Field"|]; Code[10])
        {
            TableRelation = "My Table";
        }
    }

    keys
    {
        key(PK; MyField) { }
    }
}