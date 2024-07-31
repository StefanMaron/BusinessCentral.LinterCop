codeunit 50100 MyCodeunit
{
    procedure MyProcedure(Param: Boolean)
    begin
        if Param then begin
            [|;|]
            Message('Hello');
        end;
        [|;|]
    end;
}