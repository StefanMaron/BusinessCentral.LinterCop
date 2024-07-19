table 50100 MyTable
{
    fields
    {
        // This test is to exclude the diagnostic on Journal Template tables
        // To mimic this, the Primary Key field is named "Name"
        [|field(1; Name; Code[20])|]
        {
            NotBlank = true;
        }
        field(2; "No. Series"; Code[20])
        {
            TableRelation = "No. Series"."Code";
        }
    }

    keys
    {
        key(Key1; Name)
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