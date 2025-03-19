// [|enum|] 50100 "My Enum"
// {
//     [|Access|] = [|Public|];
//     [|AssignmentCompatibility|] = [|true|];
//     [|AssignmentCompatibilityReason|] = 'My AssignmentCompatibilityReason';
//     [|Caption|] = 'My Caption';
//     [|Extensible|] = [|true|];
//     [|ObsoleteReason|] = 'My ObsoleteReason';
//     [|ObsoleteState|] = [|Pending|];
//     [|ObsoleteTag|] = 'My ObsoleteTag';
//     [|Scope|] = [|Cloud|];

//     [|value|](0; MyValue)
//     {
//         [|Caption|] = 'My value', [|Comment|] = 'My Comment', [|Locked|] = [|false|], [|MaxLength|] = 100;
//         [|ObsoleteReason|] = 'My ObsoleteReason';
//         [|ObsoleteState|] = [|Pending|];
//         [|ObsoleteTag|] = 'My ObsoleteTag';
//     }
// }

// [|enum|] 50101 "My Interface Enum" [|implements|] [|"My Interface"|]
// {
//     [|DefaultImplementation|] = [|"My Interface"|] = [|"My Codeunit"|];
//     [|UnknownValueImplementation|] = [|"My Interface"|] = [|"My Codeunit"|];

//     [|value|](0; MyValue)
//     {
//         [|Implementation|] = [|"My Interface"|] = [|"My Codeunit"|];
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