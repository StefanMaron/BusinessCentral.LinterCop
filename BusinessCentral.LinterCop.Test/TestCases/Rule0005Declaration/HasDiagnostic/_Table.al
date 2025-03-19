// [|TABLE|] 50000 "My Table"
// {
//     [|ACCESS|] = [|PUBLIC|];
//     [|CAPTION|] = 'MY TABLE';
//     [|COLUMNSTOREINDEX|] = [|"MY FIELD"|];
//     [|COMPRESSIONTYPE|] = [|UNSPECIFIED|];
//     [|DATACAPTIONFIELDS|] = [|"MY FIELD"|];
//     [|DATACLASSIFICATION|] = [|ENDUSERIDENTIFIABLEINFORMATION|];
//     [|DATAPERCOMPANY|] = [|FALSE|];
//     [|DESCRIPTION|] = 'MY DESCRIPTION';
//     [|EXTENSIBLE|] = [|TRUE|];
//     [|DRILLDOWNPAGEID|] = [|"MY PAGE"|];
//     [|INHERENTENTITLEMENTS|] = RIMDX;
//     [|INHERENTPERMISSIONS|] = RIMDX;
//     [|LOOKUPPAGEID|] = [|"MY PAGE"|];
//     [|OBSOLETEREASON|] = 'MY OBSOLETEREASON';
//     [|OBSOLETESTATE|] = [|PENDING|];
//     [|OBSOLETETAG|] = 'MY OBSOLETETAG';
//     [|REPLICATEDATA|] = [|FALSE|];
//     [|SCOPE|] = [|CLOUD|];
//     [|TABLETYPE|] = [|NORMAL|];

//     [|FIELDS|]
//     {
//         [|FIELD|](1; "My Field"; [|INTEGER|])
//         {
//             [|ACCESS|] = [|PROTECTED|];
//             [|ACCESSBYPERMISSION|] = [|TABLE|] [|"MY TABLE"|] = X;
//             [|ALLOWINCUSTOMIZATIONS|] = [|ALWAYS|];
//             [|AUTOFORMATEXPRESSION|] = '<CURRENCY CODE>';
//             [|AUTOFORMATTYPE|] = 2;
//             [|AUTOINCREMENT|] = [|FALSE|];
//             [|BLANKNUMBERS|] = [|BLANKZERO|];
//             [|BLANKZERO|] = [|FALSE|];
//             [|CAPTION|] = 'MY CAPTION';
//             [|DATACLASSIFICATION|] = [|SYSTEMMETADATA|];
//             [|DESCRIPTION|] = 'MY DESCRIPTION';
//             [|EDITABLE|] = [|FALSE|];
//             [|ENABLED|] = [|TRUE|];
//             [|FIELDCLASS|] = [|NORMAL|];
//             [|INITVALUE|] = 1;
//             [|MAXVALUE|] = 100;
//             [|MINVALUE|] = 1;
//             [|NOTBLANK|] = [|FALSE|];
//         }
//         [|FIELD|](2; "My FlowField"; [|INTEGER|])
//         {
//             [|FIELDCLASS|]= [|FLOWFIELD|];
//             [|CALCFORMULA|] = [|MAX|]([|"MY TABLE"|].[|"MY FIELD"|] [|WHERE|]([|"MY FIELD"|] = [|FIELD|]([|"MY FIELD"|])));
//         }
//         [|FIELD|](3; "My Blob"; [|BLOB|])
//         {
//             [|SUBTYPE|] = [|JSON|];
//         }
//     }

//     [|FIELDGROUPS|]
//     {
//         [|FIELDGROUP|](DropDown; [|"MY FIELD"|])
//         {
//             [|CAPTION|] = 'My DropDown';
//             [|OBSOLETEREASON|] = 'My ObsoleteReason';
//             [|OBSOLETESTATE|] = [|PENDING|];
//             [|OBSOLETETAG|] = 'My ObsoleteTag';
//         }
//         [|FIELDGROUP|](Brick; [|"MY FIELD"|], [|"My FLOWFIELD"|])
//         {
//             [|OBSOLETESTATE|] = [|NO|];
//         }
//     }

//     [|TRIGGER|] [|ONINSERT|]()
//     [|BEGIN|]
//     [|END|];

//     [|TRIGGER|] [|ONMODIFY|]()
//     [|BEGIN|]
//     [|END|];

//     [|TRIGGER|] [|ONDELETE|]()
//     [|BEGIN|]
//     [|END|];

//     [|TRIGGER|] [|ONRENAME|]()
//     [|BEGIN|]
//     [|END|];
// }

// page 50000 "My Page" { }