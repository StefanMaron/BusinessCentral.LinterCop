codeunit 50100 MyCodeunit
{
    procedure [|MyProcedure|]()     // Cognitive Complexity: 15 (threshold >=15)
    var
        Condition: Boolean;
    begin
        MyProcedure();          // +1 (nesting = 0)
        if true then            // +1 (nesting = 0)
            if true then        // +2 (nesting = 1)
                MyProcedure();  // +1 (no nesting penalty on recursion)

        while true do           // +1 (nesting = 0)
            MyProcedure();      // +1 (no nesting penalty on recursion)

        if true then            // +1 (nesting = 0)
            if Condition then   // +2 (nesting = 1)
                MyProcedure()   // +1 (no nesting penalty on recursion)
            else                // +2 (nesting = 1)
                MyProcedure();  // +1 (no nesting penalty on recursion)

        repeat                  // +1 (nesting = 0)
            this.MyProcedure(); // +1 (no nesting penalty on recursion)
        until true;
    end;
}