codeunit 50000 MyCodeunit
{
    var
        myAction: [|Action|];
        myBigInteger: [|BigInteger|];
        myBigText: [|BigText|];
        myBoolean: [|Boolean|];
        myByte: [|Byte|];
        myChar: [|Char|];
        myClientType: [|ClientType|];
        myDataScope: [|DataScope|];
        myDataTransfer: [|DataTransfer|];
        myDate: [|Date|];
        myDateFormula: [|DateFormula|];
        myDateTime: [|DateTime|];
        myDecimal: [|Decimal|];
        myDialog: [|Dialog|];
        myDuration: [|Duration|];
        myErrorInfo: [|ErrorInfo|];
        myFieldRef: [|FieldRef|];
        myFieldType: [|FieldType|];
        myFile: [|File|];
        myFileUpload: [|FileUpload|];
        myFilterPageBuilder: [|FilterPageBuilder|];
        myGuid: [|Guid|];
        myHttpClient: [|HttpClient|];
        myHttpContent: [|HttpContent|];
        myHttpHeaders: [|HttpHeaders|];
        myHttpRequestMessage: [|HttpRequestMessage|];
        myHttpResponseMessage: [|HttpResponseMessage|];
        myInStream: [|InStream|];
        myInteger: [|Integer|];
        myIsolationLevel: [|IsolationLevel|];
        myJsonArray: [|JsonArray|];
        myJsonObject: [|JsonObject|];
        myJsonToken: [|JsonToken|];
        myJsonValue: [|JsonValue|];
        myKeyRef: [|KeyRef|];
        myModuleInfo: [|ModuleInfo|];
        myNotification: [|Notification|];
        myNotificationScope: [|NotificationScope|];
        myObjectType: [|ObjectType|];
        myOutStream: [|OutStream|];
        myPromptMode: [|PromptMode|];
        myRecordId: [|RecordId|];
        myRecordRef: [|RecordRef|];
        myReportFormat: [|ReportFormat|];
        myReportLayoutType: [|ReportLayoutType|];
        mySecretText: [|SecretText|];
        mySecurityFilter: [|SecurityFilter|];
        mySessionSettings: [|SessionSettings|];
        myTableConnectionType: [|TableConnectionType|];
        myTelemetryScope: [|TelemetryScope|];
        myTestPermissions: [|TestPermissions|];
        myTextBuilder: [|TextBuilder|];
        myTextEncoding: [|TextEncoding|];
        myTime: [|Time|];
        myVariant: [|Variant|];
        myVersion: [|Version|];
        myXmlAttribute: [|XmlAttribute|];
        myXmlDeclaration: [|XmlDeclaration|];
        myXmlDocument: [|XmlDocument|];
        myXmlElement: [|XmlElement|];
        myXmlNamespaceManager: [|XmlNamespaceManager|];
        myXmlNode: [|XmlNode|];
        myXmlNodeList: [|XmlNodeList|];
        myXmlWriteOptions: [|XmlWriteOptions|];
}

table 50000 MyTable
{
    fields
    {
        field(1; "Primary Key"; Integer) { }
        field(2; myBigInteger; [|BigInteger|]) { }
        field(3; myBlob; [|Blob|]) { }
        field(4; myBoolean; [|Boolean|]) { }
        field(5; myDate; [|Date|]) { }
        field(6; myDateFormula; [|DateFormula|]) { }
        field(7; myDateTime; [|DateTime|]) { }
        field(8; myDecimal; [|Decimal|]) { }
        field(9; myDuration; [|Duration|]) { }
        field(10; myGuid; [|Guid|]) { }
        field(11; myInteger; [|Integer|]) { }
        field(12; myMedia; [|Media|]) { }
        field(13; myMediaSet; [|MediaSet|]) { }
        field(14; myRecordId; [|RecordId|]) { }
        field(15; myTableFilter; [|TableFilter|]) { }
        field(16; myTime; [|Time|]) { }
    }
}