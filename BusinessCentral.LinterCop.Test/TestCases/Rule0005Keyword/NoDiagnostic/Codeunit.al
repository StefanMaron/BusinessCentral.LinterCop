[|codeunit|] 50100 "My Codeunit"
{
    Permissions = [|tabledata|] "My Table" = RIMD;
    SingleInstance = [|true|];

    [|trigger|] OnRun()
    [|begin|]
    [|end|];

    [EventSubscriber(ObjectType::Table, Database::"My Table", OnAfterInsertEvent, '', [|false|], [|false|])]
    [|local|] [|procedure|] OnAfterInsertEvent([|var|] Rec: RECORD "MY TABLE")
    [|begin|]
    [|end|];

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"My Codeunit", MyBusinessEvent, '', [|true|], [|true|])]
    [|local|] [|procedure|] MyBusinessEventSubscriber(sender: Codeunit "My Codeunit")
    [|begin|]
    [|end|];

    [ExternalBusinessEvent('MyExternalBusinessEvent', 'My ExternalBusinessEvent', 'My External Business Event', EventCategory::"My Event", '1.0')]
    [|local|] [|procedure|] MyExternalBusinessEvent()
    [|begin|]
    [|end|];

    [IntegrationEvent([|false|], [|false|])]
    [|local|] [|procedure|] MyIntegrationEvent()
    [|begin|]
    [|end|];

    [BusinessEvent([|true|])]
    [|local|] [|procedure|] MyBusinessEvent()
    [|begin|]
    [|end|];

    [|var|]
        MyTable: Record "My Table" [|temporary|];
        MyLabel: Label 'My Label',  Comment = 'My Comment', Locked = [|true|], MaxLength = 100;

    [|procedure|] MyProcedure(MyTable: Record "My Table" [|temporary|])
    [|begin|]
    [|end|];
}

table 50100 "My Table" { fields { field(1; "My field"; Code[10]) { } } }
enum 2000000001 EventCategory { Extensible = true; }
enumextension 50100 MyEventCategory extends EventCategory { value(50100; "My Event") { } }