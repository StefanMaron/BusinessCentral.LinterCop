[|TABLE|] 50000 "My Table"
{
    ReplicateData = [|FALSE|];

    [|FIELDS|]
    {
        [|FIELD|](1; "My Field"; Integer)
        {
            AccessByPermission = [|TABLE|] "My Table" = X;
            Editable = [|FALSE|];
            NotBlank = [|FALSE|];
        }
        [|FIELD|](2; "My FlowField"; Integer)
        {
            FieldClass= FlowField;
            CalcFormula = [|MAX|]("My Table"."My Field" [|WHERE|]("My Field" = [|FIELD|]("My Field")));
        }
    }

    [|FIELDGROUPS|]
    {
        [|FIELDGROUP|](DropDown; "My Field") { }
        [|FIELDGROUP|](Brick; "My Field", "My FlowField") { }
    }

    [|TRIGGER|] OnInsert()
    [|BEGIN|]
    [|END|];
    
    [|TRIGGER|] OnModify()
    [|BEGIN|]
    [|END|];
    
    [|TRIGGER|] OnDelete()
    [|BEGIN|]
    [|END|];
    
    [|TRIGGER|] OnRename()
    [|BEGIN|]
    [|END|];
}