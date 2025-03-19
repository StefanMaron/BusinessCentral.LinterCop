// [|CODEUNIT|] 50100 "My Codeunit"
// {
//     [|ACCESS|] = [|PUBLIC|];
//     [|DESCRIPTION|] = 'MY DESCRIPTION';
//     [|EVENTSUBSCRIBERINSTANCE|] = [|STATICAUTOMATIC|];
//     [|INHERENTENTITLEMENTS|] = X;
//     [|INHERENTPERMISSIONS|] = X;
//     [|PERMISSIONS|] = [|TABLEDATA|] [|"MY TABLE"|] = RIMD;
//     [|SINGLEINSTANCE|] = [|TRUE|];
//     [|SUBTYPE|] = [|NORMAL|];
//     [|TABLENO|] = [|"MY TABLE"|];

//     [|TRIGGER|] [|ONRUN|]()
//     [|BEGIN|]
//     [|END|];

//     [[|EVENTSUBSCRIBER|]([|OBJECTTYPE|]::[|TABLE|], [|DATABASE|]::[|"MY TABLE"|], [|ONAFTERINSERTEVENT|], '', [|FALSE|], [|FALSE|])]
//     [|LOCAL|] [|PROCEDURE|] OnAfterInsertEvent([|VAR|] Rec: [|RECORD|] [|"MY TABLE"|]) //TODO: Rec
//     [|BEGIN|]
//     [|END|];

//     [[|EVENTSUBSCRIBER|]([|OBJECTTYPE|]::[|CODEUNIT|], [|CODEUNIT|]::[|"MY CODEUNIT"|], [|MYBUSINESSEVENT|], '', [|TRUE|], [|TRUE|])]
//     [|LOCAL|] [|PROCEDURE|] MyBusinessEventSubscriber(SENDER: [|CODEUNIT|] [|"MY CODEUNIT"|])
//     [|BEGIN|]
//     [|END|];

//     [[|EXTERNALBUSINESSEVENT|]('MyExternalBusinessEvent', 'My ExternalBusinessEvent', 'My External Business Event', [|EVENTCATEGORY|]::[|MYEVENT|], '1.0')]
//     [|LOCAL|] [|PROCEDURE|] MyExternalBusinessEvent()
//     [|BEGIN|]
//     [|END|];

//     [[|INTEGRATIONEVENT|]([|FALSE|], [|FALSE|])]
//     [|LOCAL|] [|PROCEDURE|] MyIntegrationEvent()
//     [|BEGIN|]
//     [|END|];

//     [[|BUSINESSEVENT|]([|TRUE|])]
//     [|LOCAL|] [|PROCEDURE|] MyBusinessEvent()
//     [|BEGIN|]
//     [|END|];

//     [|VAR|]
//         MyTable: [|RECORD|] [|"MY TABLE"|] [|TEMPORARY|];
//         MyEnum: [|ENUM|] [|"MY ENUM"|];
//         MyQuery: [|QUERY|] [|"MY QUERY"|];
//         MyReport: [|REPORT|] [|"MY REPORT"|];
//         MyXmlport: [|Xmlport|] [|"MY XMLPORT"|]; // The right casing here should be XmlPort (uppercase letter p)
//         MyLabel: [|LABEL|] 'My Label', [|COMMENT|] = 'My Comment', [|LOCKED|] = [|TRUE|], [|MAXLENGTH|] = 100;
//         i: [|INTEGER|];

//     [|PROCEDURE|] MyProcedure()
//     [|BEGIN|]
//         i := [|CODEUNIT|]::[|"MY CODEUNIT"|];
//         i := [|DATABASE|]::[|"MY TABLE"|];
//         i := [|ENUM|]::[|"MY ENUM"|]::[|"MY VALUE"|];;
//         i := [|PAGE|]::[|"MY PAGE"|];
//         i := [|QUERY|]::[|"MY QUERY"|];
//         i := [|REPORT|]::[|"MY REPORT"|];
//         i := [|XmlPort|]::[|"MY XMLPORT"|]; // The right casing here should be Xmlport (lowercase letter p)
//     [|END|];
// }

// enum 50100 "My Enum" { value(0; "My Value") { } }
// page 50100 "My Page" { }
// query 50100 "My Query" { elements { dataitem(MyTable; "My Table") { column(Myfield; "My field") { } } } }
// report 50100 "My Report" { }
// table 50100 "My Table" { fields { field(1; "My field"; Code[10]) { } } }
// xmlport 50100 "My Xmlport" { }
// enum 2000000001 EventCategory { Extensible = true; }
// enumextension 50100 MyEventCategory extends EventCategory { value(50100; MyEvent) { } }