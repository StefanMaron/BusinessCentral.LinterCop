// This test is to safe guard the grouping on the AnalyzeIdentifierName method
// For increasing performance on the GetSymbolInfo method by grouping nodes with the same Identifier
// Grouping needs to be case sensitive for the semanticModel.GetSymbolInfo to retrieve the correct SymbolInfo 

codeunit 50100 MyCodeunit
{
    procedure MyProcedure(myText: Text)
    begin
        myText := '';
    end;


    procedure MySecondProcedure(MyText: Text): Integer
    begin
        [|MyText|] := '';
    end;
}