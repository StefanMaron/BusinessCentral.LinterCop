codeunit 50100 MyCodeunit
{
    procedure MyProcedure([|TestOption: Option " ","Test1","Test2"|])
    begin
        case TestOption of
            TestOption::" ":
                exit;
            TestOption::"Test1":
                Message('test 1');
            TestOption::"Test2":
                Message('test 2');
        end;
    end;
}