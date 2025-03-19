codeunit 50100 MyCodeunit
{
    trigger [|OnRun|]()
    begin
    end;
}

codeunit 50101 MyInstallCodeunit
{
    Subtype = Install;

    trigger [|OnInstallAppPerDatabase|]()
    begin
    end;

    trigger [|OnInstallAppPerCompany|]()
    begin
    end;
}

codeunit 50102 MyUpgradeCodeunit
{
    Subtype = Upgrade;

    trigger [|OnCheckPreconditionsPerDatabase|]()
    begin
    end;

    trigger [|OnCheckPreconditionsPerCompany|]()
    begin
    end;

    trigger [|OnUpgradePerDatabase|]()
    begin
    end;

    trigger [|OnUpgradePerCompany|]()
    begin
    end;

    trigger [|OnValidateUpgradePerDatabase|]()
    begin
    end;

    trigger [|OnValidateUpgradePerCompany|]()
    begin
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer)
        {
            trigger [|OnLookup|]()
    begin
    end;

            trigger [|OnValidate|]()
            begin
            end;
        }
    }

    trigger [|OnInsert|]()
    begin
    end;

    trigger [|OnModify|]()
    begin
    end;

    trigger [|OnDelete|]()
    begin
    end;

    trigger [|OnRename|]()
    begin
    end;
}

tableextension 50100 MyTableExtension extends MyTable
{
    fields
    {
        modify(MyField)
        {
            trigger [|OnBeforeValidate|]()
    begin
    end;

            trigger [|OnAfterValidate|]()
            begin
            end;
        }
    }

    trigger [|OnBeforeInsert|]()
    begin
    end;

    trigger [|OnInsert|]()
    begin
    end;

    trigger [|OnAfterInsert|]()
    begin
    end;

    trigger [|OnBeforeModify|]()
    begin
    end;

    trigger [|OnModify|]()
    begin
    end;

    trigger [|OnAfterModify|]()
    begin
    end;

    trigger [|OnBeforeDelete|]()
    begin
    end;

    trigger [|OnDelete|]()
    begin
    end;

    trigger [|OnAfterDelete|]()
    begin
    end;

    trigger [|OnRename|]()
    begin
    end;

    trigger [|OnBeforeRename|]()
    begin
    end;

    trigger [|OnAfterRename|]()
    begin
    end;
}

report 50100 MyReport
{
    dataset
    {
        dataitem(MyDataItemName; MyTable)
        {
            trigger [|OnPreDataItem|]()
    begin
    end;

            trigger [|OnAfterGetRecord|]()
            begin
            end;

            trigger [|OnPostDataItem|]()
            begin
            end;
        }
    }

    trigger [|OnInitReport|]()
    begin
    end;

    trigger [|OnPreReport|]()
    begin
    end;

    trigger [|OnPostReport|]()
    begin
    end;
}

page 50100 MyPage
{
    SourceTable = MyTable;

    layout
    {
        area(Content)
        {
            field(MyField; Rec.MyField)
            {
                ApplicationArea = All;
            }
        }
    }
}

pageextension 50100 MyPageExtension extends MyPage
{
    layout
    {
        addafter(MyField)
        {
            field(MyFieldExt; Rec.MyField)
            {
                ApplicationArea = All;
                trigger [|OnDrillDown|]()
    begin
    end;
}
        }
    }
}