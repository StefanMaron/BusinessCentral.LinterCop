codeunit 50100 MyCodeunit
{
    procedure [|MyProcedure|](Day: Integer) // Cognitive Complexity: 15 (threshold >=15)
    var
        DayName: Text;
    begin
        DayName := (Day = 1) ? 'Monday' :      // +1 (nested = 0)
               (Day = 2) ? 'Tuesday' :         // +2 (nested = 1)
               (Day = 3) ? 'Wednesday' :       // +3 (nested = 2)
               (Day = 4) ? 'Thursday' :        // +4 (nested = 3)
               (Day = 5) ? 'Friday' :          // +5 (nested = 4)
               'Invalid Day';
    end;
}