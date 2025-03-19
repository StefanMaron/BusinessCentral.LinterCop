codeunit 50100 "My Codeunit"
{
    [|Access|] = [|Public|];
    [|Description|] = 'My Description';
    [|EventSubscriberInstance|] = [|StaticAutomatic|];
    [|InherentEntitlements|] = X;
    [|InherentPermissions|] = X;
    [|Permissions|] = tabledata [|"My Table"|] = RIMD;
    [|SingleInstance|] = true;
    [|Subtype|] = [|Normal|];
    [|TableNo|] = [|"My Table"|];
}

table 50100 "My Table"
{
    [|Access|] = [|Public|];
    [|Caption|] = 'My Caption';
    [|ColumnStoreIndex|] = [|"My Field"|];
    [|CompressionType|] = [|Unspecified|];
    [|DataCaptionFields|] = [|"My Field"|];
    [|DataClassification|] = [|EndUserIdentifiableInformation|];
    [|DataPerCompany|] = false;
    [|Description|] = 'My Description';
    [|Extensible|] = true;
    [|DrillDownPageId|] = [|"My Page"|];
    [|InherentEntitlements|] = RIMDX;
    [|InherentPermissions|] = RIMDX;
    [|LookupPageId|] = [|"My Page"|];
    [|ObsoleteReason|] = 'My ObsoleteReason';
    [|ObsoleteState|] = [|Pending|];
    [|ObsoleteTag|] = 'My ObsoleteTag';
    [|ReplicateData|] = false;
    [|Scope|] = [|Cloud|];
    [|TableType|] = [|Normal|];

    fields
    {
        field(1; "My Field"; Integer)
        {
            [|Access|] = [|Protected|];
            [|AccessByPermission|] = table [|"My Table"|] = X;
            [|AllowInCustomizations|] = [|Always|];
            [|AutoFormatExpression|] = '<Currency Code>';
            [|AutoFormatType|] = 2;
            [|AutoIncrement|] = false;
            [|BlankNumbers|] = [|BlankZero|];
            [|BlankZero|] = false;
            [|Caption|] = 'My Caption';
            [|DataClassification|] = [|SystemMetadata|];
            [|Description|] = 'My Description';
            [|Editable|] = false;
            [|Enabled|] = true;
            [|FieldClass|] = [|Normal|];
            [|InitValue|] = 1;
            [|MaxValue|] = 100;
            [|MinValue|] = 1;
            [|NotBlank|] = false;
        }
        field(2; "My FlowField"; Integer)
        {
            [|FieldClass|] = FlowField;
            [|CalcFormula|] = max([|"My Table"|].[|"My Field"|] where([|"My Field"|] = field([|"My Field"|])));
        }
        field(3; "My Blob"; Blob)
        {
            [|Subtype|] = [|Json|];
        }
    }

    fieldgroups
    {
        fieldgroup(DropDown; "My Field")
        {
            [|Caption|] = 'My DropDown';
            [|ObsoleteReason|] = 'My ObsoleteReason';
            [|ObsoleteState|] = [|Pending|];
            [|ObsoleteTag|] = 'My ObsoleteTag';
        }
        fieldgroup(Brick; "My Field", "My FlowField")
        {
            [|ObsoleteState|] = [|No|];
        }
    }
}

page 50100 "My Page"
{
    [|ApplicationArea|] = All;
    [|AboutText|] = 'My AboutText';
    [|AboutTitle|] = 'My AboutTitle';
    [|AccessByPermission|] = table [|"My Table"|] = X;
    [|AdditionalSearchTerms|] = 'My AdditionalSearchTerms';
    [|UsageCategory|] = [|Administration|];
}

page 50101 "My Api Page"
{
    [|ApplicationArea|] = All;
    [|APIGroup|] = 'myAPIGroup';
    [|APIPublisher|] = 'myAPIPublisher';
    [|APIVersion|] = 'v1.0';
    [|DataAccessIntent|] = [|ReadOnly|];
    [|DelayedInsert|] = true;
    [|Editable|] = false;
    [|EntityName|] = 'myEntityName';
    [|EntitySetName|] = 'myEntitySetName';
    [|PageType|] = [|API|];
}

enum 50100 "My Enum"
{
    [|Access|] = [|Public|];
    [|AssignmentCompatibility|] = true;
    [|AssignmentCompatibilityReason|] = 'My AssignmentCompatibilityReason';
    [|Caption|] = 'My Caption';
    [|Extensible|] = true;
    [|ObsoleteReason|] = 'My ObsoleteReason';
    [|ObsoleteState|] = [|Pending|];
    [|ObsoleteTag|] = 'My ObsoleteTag';
    [|Scope|] = [|Cloud|];

    value(0; MyValue)
    {
        [|Caption|] = 'My value', [|Comment|] = 'My Comment', [|Locked|] = false, [|MaxLength|] = 100;
        [|ObsoleteReason|] = 'My ObsoleteReason';
        [|ObsoleteState|] = [|Pending|];
        [|ObsoleteTag|] = 'My ObsoleteTag';
    }
}

enum 50101 "My Interface Enum" implements "My Interface"
{
    
    [|DefaultImplementation = [|"My Interface"|] = [|"My Interface Codeunit"|];
    [|UnknownValueImplementation = [|"My Interface"|] = [|"My Interface Codeunit"|];

    value(0; "My Value")
    {
        [|Implementation|] = [|"My Interface"|] = [|"My Interface Codeunit"|];
    }
}

interface "My Interface"
{
    procedure MyProcedure();
}

codeunit 50101 "My Interface Codeunit" implements "My Interface"
{
    procedure MyProcedure()
    begin
    end;
}