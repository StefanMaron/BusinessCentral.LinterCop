table 50100 MyTable
{
    fields
    {
        [|field(1; MyField; Code[20])|] { }
        field(2; "No. Series"; Code[20])
        {
            TableRelation = "No. Series";
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

table 308 "No. Series"
{
    fields
    {
        field(1; "Code"; Code[20])
        {
            Caption = 'Code';
            NotBlank = true;
        }
    }
}