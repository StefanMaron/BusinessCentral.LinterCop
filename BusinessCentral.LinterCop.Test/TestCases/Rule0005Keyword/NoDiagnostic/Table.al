[|table|] 50000 "My Table"
{
    ReplicateData = [|false|];

    [|fields|]
    {
        [|field|](1; "My Field"; Integer)
        {
            AccessByPermission = [|table|] "My Table" = X;
            Editable = [|false|];
            NotBlank = [|false|];
        }
        [|field|](2; "My FlowField"; Integer)
        {
            FieldClass= FlowField;
            CalcFormula = [|max|]("My Table"."My Field" [|where|]("My Field" = [|field|]("My Field")));
        }
    }

    [|fieldgroups|]
    {
        [|fieldgroup|](DropDown; "My Field") { }
        [|fieldgroup|](Brick; "My Field", "My FlowField") { }
    }

    [|trigger|] OnInsert()
    [|begin|]
    [|end|];
    
    [|trigger|] OnModify()
    [|begin|]
    [|end|];
    
    [|trigger|] OnDelete()
    [|begin|]
    [|end|];
    
    [|trigger|] OnRename()
    [|begin|]
    [|end|];
}

page 50000 "My Page" { }