xmlport 50100 "My Xmlport"
{
    schema
    {
        textelement(NodeName1)
        {
            tableelement(NodeName2; "My Table")
            {
                fieldattribute(NodeName3; [|NODENAME2|]."My field") { }
            }
        }
    }
}

table 50100 "My Table" { fields { field(1; "My field"; Code[10]) { } } }
