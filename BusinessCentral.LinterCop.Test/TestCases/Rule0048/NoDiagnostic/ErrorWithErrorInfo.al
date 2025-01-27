codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyErrorInfo: ErrorInfo;
    begin
        MyErrorInfo := ErrorInfo.Create(GetLastErrorText());
        [|Error(MyErrorInfo)|];
    end;
}