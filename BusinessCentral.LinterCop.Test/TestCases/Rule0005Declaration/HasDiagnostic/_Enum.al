// [|ENUM|] 50100 "My Enum"
// {
//     [|ACCESS|] = [|PUBLIC|];
//     [|ASSIGNMENTCOMPATIBILITY|] = [|TRUE|];
//     [|ASSIGNMENTCOMPATIBILITYREASON|] = 'My AssignmentCompatibilityReason';
//     [|CAPTION|] = 'My Caption';
//     [|EXTENSIBLE|] = [|TRUE|];
//     [|OBSOLETEREASON|] = 'My ObsoleteReason';
//     [|OBSOLETESTATE|] = [|PENDING|];
//     [|OBSOLETETAG|] = 'My ObsoleteTag';
//     [|SCOPE|] = [|CLOUD|];

//     [|VALUE|](0; MyValue)
//     {
//         [|CAPTION|] = 'My value', [|COMMENT|] = 'My Comment', [|LOCKED|] = [|FALSE|], [|MAXLENGTH|] = 100;
//         [|OBSOLETEREASON|] = 'My ObsoleteReason';
//         [|OBSOLETESTATE|] = [|PENDING|];
//         [|OBSOLETETAG|] = 'My ObsoleteTag';
//     }
// }

// [|ENUM|] 50101 "My Interface Enum" [|IMPLEMENTS|] [|"MY INTERFACE"|]
// {
//     [|DEFAULTIMPLEMENTATION|] = [|"MY INTERFACE"|] = [|"MY CODEUNIT"|];
//     [|UNKNOWNVALUEIMPLEMENTATION|] = [|"MY INTERFACE"|] = [|"MY CODEUNIT"|];

//     [|VALUE|](0; MyValue)
//     {
//         [|IMPLEMENTATION|] = [|"MY INTERFACE"|] = [|"MY CODEUNIT"|];
//     }
// }

// interface "My Interface"
// {
//     procedure MyProcedure();
// }

// codeunit 50100 "My Codeunit" implements "My Interface"
// {
//     procedure MyProcedure()
//     begin
//     end;
// }