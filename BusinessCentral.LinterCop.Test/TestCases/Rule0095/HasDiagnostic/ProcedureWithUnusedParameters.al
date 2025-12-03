codeunit 70020 MyCodeunit
{
    internal procedure TestProcedure([|NotUsed|]: Text; Used: Text; [|NotUsed2|]: Text; Used2: Text)
    begin
        Used := '42';
        Used2 := '42';
    end;
}