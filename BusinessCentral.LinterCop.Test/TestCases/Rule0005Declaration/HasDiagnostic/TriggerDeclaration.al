codeunit 50100 MyCodeunit
{
    trigger [|ONRUN|]()
    begin
    end;
}

codeunit 50101 MyInstallCodeunit
{
    Subtype = Install;

    trigger [|ONINSTALLAPPPERDATABASE|]()
    begin
    end;

    trigger [|ONINSTALLAPPPERCOMPANY|]()
    begin
    end;
}

codeunit 50102 MyUpgradeCodeunit
{
    Subtype = Upgrade;

    trigger [|ONCHECKPRECONDITIONSPERDATABASE|]()
    begin
    end;

    trigger [|ONCHECKPRECONDITIONSPERCOMPANY|]()
    begin
    end;

    trigger [|ONUPGRADEPERDATABASE|]()
    begin
    end;

    trigger [|ONUPGRADEPERCOMPANY|]()
    begin
    end;

    trigger [|ONVALIDATEUPGRADEPERDATABASE|]()
    begin
    end;

    trigger [|ONVALIDATEUPGRADEPERCOMPANY|]()
    begin
    end;
}

table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer)
        {
            trigger [|ONLOOKUP|]()
    begin
    end;

            trigger [|ONVALIDATE|]()
            begin
            end;
        }
    }

    trigger [|ONINSERT|]()
    begin
    end;

    trigger [|ONMODIFY|]()
    begin
    end;

    trigger [|ONDELETE|]()
    begin
    end;

    trigger [|ONRENAME|]()
    begin
    end;
}

tableextension 50100 MyTableExtension extends MyTable
{
    fields
    {
        modify(MyField)
        {
            trigger [|ONBEFOREVALIDATE|]()
    begin
    end;

            trigger [|ONAFTERVALIDATE|]()
            begin
            end;
        }
    }

    trigger [|ONBEFOREINSERT|]()
    begin
    end;

    trigger [|ONINSERT|]()
    begin
    end;

    trigger [|ONAFTERINSERT|]()
    begin
    end;

    trigger [|ONBEFOREMODIFY|]()
    begin
    end;

    trigger [|ONMODIFY|]()
    begin
    end;

    trigger [|ONAFTERMODIFY|]()
    begin
    end;

    trigger [|ONBEFOREDELETE|]()
    begin
    end;

    trigger [|ONDELETE|]()
    begin
    end;

    trigger [|ONAFTERDELETE|]()
    begin
    end;

    trigger [|ONRENAME|]()
    begin
    end;

    trigger [|ONBEFORERENAME|]()
    begin
    end;

    trigger [|ONAFTERRENAME|]()
    begin
    end;
}

report 50100 MyReport
{
    dataset
    {
        dataitem(MyDataItemName; MyTable)
        {
            trigger [|ONPREDATAITEM|]()
    begin
    end;

            trigger [|ONAFTERGETRECORD|]()
            begin
            end;

            trigger [|ONPOSTDATAITEM|]()
            begin
            end;
        }
    }

    trigger [|ONINITREPORT|]()
    begin
    end;

    trigger [|ONPREREPORT|]()
    begin
    end;

    trigger [|ONPOSTREPORT|]()
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
                trigger [|ONDRILLDOWN|]()
    begin
    end;
}
        }
    }
}