codeunit 50100 "My Codeunit"
{
    [|ACCESS|] = [|PUBLIC|];
    [|DESCRIPTION|] = 'My Description';
    [|EVENTSUBSCRIBERINSTANCE|] = [|STATICAUTOMATIC|];
    [|INHERENTENTITLEMENTS|] = X;
    [|INHERENTPERMISSIONS|] = X;
    [|PERMISSIONS|] = tabledata [|"MY TABLE"|] = RIMD;
    [|SINGLEINSTANCE|] = true;
    [|SUBTYPE|] = [|NORMAL|];
    [|TABLENO|] = [|"MY TABLE"|];
}

table 50100 "My Table"
{
    [|ACCESS|] = [|PUBLIC|];
    [|CAPTION|] = 'My Caption';
    [|COLUMNSTOREINDEX|] = [|"MY FIELD"|];
    [|COMPRESSIONTYPE|] = [|UNSPECIFIED|];
    [|DATACAPTIONFIELDS|] = [|"MY FIELD"|];
    [|DATACLASSIFICATION|] = [|ENDUSERIDENTIFIABLEINFORMATION|];
    [|DATAPERCOMPANY|] = false;
    [|DESCRIPTION|] = 'My Description';
    [|EXTENSIBLE|] = true;
    [|DRILLDOWNPAGEID|] = [|"MY PAGE"|];
    [|INHERENTENTITLEMENTS|] = RIMDX;
    [|INHERENTPERMISSIONS|] = RIMDX;
    [|LOOKUPPAGEID|] = [|"MY PAGE"|];
    [|OBSOLETEREASON|] = 'My ObsoleteReason';
    [|OBSOLETESTATE|] = [|PENDING|];
    [|OBSOLETETAG|] = 'My ObsoleteTag';
    [|REPLICATEDATA|] = false;
    [|SCOPE|] = [|CLOUD|];
    [|TABLETYPE|] = [|NORMAL|];

    fields
    {
        field(1; "My Field"; Integer)
        {
            [|ACCESS|] = [|PROTECTED|];
            [|ACCESSBYPERMISSION|] = table [|"MY TABLE"|] = X;
            [|ALLOWINCUSTOMIZATIONS|] = [|ALWAYS|];
            [|AUTOFORMATEXPRESSION|] = '<Currency Code>';
            [|AUTOFORMATTYPE|] = 2;
            [|AUTOINCREMENT|] = false;
            [|BLANKNUMBERS|] = [|BLANKZERO|];
            [|BLANKZERO|] = false;
            [|CAPTION|] = 'My Caption';
            [|DATACLASSIFICATION|] = [|SYSTEMMETADATA|];
            [|DESCRIPTION|] = 'My Description';
            [|EDITABLE|] = false;
            [|ENABLED|] = true;
            [|FIELDCLASS|] = [|NORMAL|];
            [|INITVALUE|] = 1;
            [|MAXVALUE|] = 100;
            [|MINVALUE|] = 1;
            [|NOTBLANK|] = false;
        }
        field(2; "My FlowField"; Integer)
        {
            [|FIELDCLASS|] = FLOWFIELD;
            [|CALCFORMULA|] = max([|"MY TABLE"|].[|"MY FIELD"|] where([|"MY FIELD"|] = field([|"MY FIELD"|])));
        }
        field(3; "My Blob"; Blob)
        {
            [|SUBTYPE|] = [|JSON|];
        }
    }

    fieldgroups
    {
        fieldgroup(DropDown; "My Field")
        {
            [|CAPTION|] = 'My DropDown';
            [|OBSOLETEREASON|] = 'My ObsoleteReason';
            [|OBSOLETESTATE|] = [|PENDING|];
            [|OBSOLETETAG|] = 'My ObsoleteTag';
        }
        fieldgroup(Brick; "My Field", "My FlowField")
        {
            [|OBSOLETESTATE|] = [|NO|];
        }
    }
}

page 50100 "My Page"
{
    [|APPLICATIONAREA|] = All;
    [|ABOUTTEXT|] = 'My AboutText';
    [|ABOUTTITLE|] = 'My AboutTitle';
    [|ACCESSBYPERMISSION|] = table [|"MY TABLE"|] = X;
    [|ADDITIONALSEARCHTERMS|] = 'My AdditionalSearchTerms';
    [|USAGECATEGORY|] = [|ADMINISTRATION|];
}

page 50101 "My Api Page"
{
    [|APPLICATIONAREA|] = All;
    [|APIGROUP|] = 'myAPIGroup';
    [|APIPUBLISHER|] = 'myAPIPublisher';
    [|APIVERSION|] = 'v1.0';
    [|DATAACCESSINTENT|] = [|READONLY|];
    [|DELAYEDINSERT|] = true;
    [|EDITABLE|] = false;
    [|ENTITYNAME|] = 'myEntityName';
    [|ENTITYSETNAME|] = 'myEntitySetName';
    [|PAGETYPE|] = [|api|]; // UpperCase is already the correct casing
}

enum 50100 "My Enum"
{
    [|ACCESS|] = [|PUBLIC|];
    [|ASSIGNMENTCOMPATIBILITY|] = true;
    [|ASSIGNMENTCOMPATIBILITYREASON|] = 'My AssignmentCompatibilityReason';
    [|CAPTION|] = 'My Caption';
    [|EXTENSIBLE|] = true;
    [|OBSOLETEREASON|] = 'My ObsoleteReason';
    [|OBSOLETESTATE|] = [|PENDING|];
    [|OBSOLETETAG|] = 'My ObsoleteTag';
    [|SCOPE|] = [|CLOUD|];

    value(0; MyValue)
    {
        [|CAPTION|] = 'My value', [|COMMENT|] = 'My Comment', [|LOCKED|] = false, [|MAXLENGTH|] = 100;
        [|OBSOLETEREASON|] = 'My ObsoleteReason';
        [|OBSOLETESTATE|] = [|PENDING|];
        [|OBSOLETETAG|] = 'My ObsoleteTag';
    }
}

enum 50101 "My Interface Enum" implements "My Interface"
{
    [|DEFAULTIMPLEMENTATION = [|"MY INTERFACE"|] = [|"MY INTERFACE CODEUNIT"|];
    [|UNKNOWNVALUEIMPLEMENTATION = [|"MY INTERFACE"|] = [|"MY INTERFACE CODEUNIT"|];

    value(0; "My Value")
    {
        [|IMPLEMENTATION|] = [|"MY INTERFACE"|] = [|"MY INTERFACE CODEUNIT"|];
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