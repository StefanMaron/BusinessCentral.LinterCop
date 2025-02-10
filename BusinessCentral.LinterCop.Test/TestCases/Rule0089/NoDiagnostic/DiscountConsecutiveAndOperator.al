codeunit 50100 MyCodeunit
{
    procedure [|MyProcedure|]() // Cognitive Complexity: 3 (threshold >=15)
    begin
        if true         // +1 (nested = 0)
            and true    // +2 (nested = 1)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
            and true    // +0 (previous is and-operator)
             then;
    end;
}