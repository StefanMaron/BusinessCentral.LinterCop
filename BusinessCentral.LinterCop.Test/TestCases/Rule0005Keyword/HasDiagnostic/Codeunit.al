[|CODEUNIT|] 50100 "My Codeunit"
{
    Permissions = [|TABLEDATA|] "My Table" = RIMD;
    SingleInstance = [|TRUE|];

    [|TRIGGER|] OnRun()
    [|BEGIN|]
    [|END|];

    [EventSubscriber(ObjectType::Table, Database::"My Table", OnAfterInsertEvent, '', [|FALSE|], [|FALSE|])]
    [|LOCAL|] [|PROCEDURE|] OnAfterInsertEvent([|VAR|] Rec: RECORD "MY TABLE")
    [|BEGIN|]
    [|END|];

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"My Codeunit", MyBusinessEvent, '', [|TRUE|], [|TRUE|])]
    [|LOCAL|] [|PROCEDURE|] MyBusinessEventSubscriber(sender: Codeunit "My Codeunit")
    [|BEGIN|]
    [|END|];

    [ExternalBusinessEvent('MyExternalBusinessEvent', 'My ExternalBusinessEvent', 'My External Business Event', EventCategory::"My Event", '1.0')]
    [|LOCAL|] [|PROCEDURE|] MyExternalBusinessEvent()
    [|BEGIN|]
    [|END|];

    [IntegrationEvent([|FALSE|], [|FALSE|])]
    [|LOCAL|] [|PROCEDURE|] MyIntegrationEvent()
    [|BEGIN|]
    [|END|];

    [BusinessEvent([|TRUE|])]
    [|LOCAL|] [|PROCEDURE|] MyBusinessEvent()
    [|BEGIN|]
    [|END|];

    [|VAR|]
        MyTable: Record "My Table" [|TEMPORARY|];
        MyLabel: Label 'My Label',  Comment = 'My Comment', Locked = [|TRUE|], MaxLength = 100;

    [|PROCEDURE|] MyProcedure(MyTable: Record "My Table" [|TEMPORARY|])
    [|BEGIN|]
    [|END|];
}

table 50100 "My Table" { fields { field(1; "My field"; Code[10]) { } } }
enum 2000000001 EventCategory { Extensible = true; }
enumextension 50100 MyEventCategory extends EventCategory { value(50100; "My Event") { } }