codeunit 50000 MyCodeunit
{
    var
        myAction: [|ACTION|];
        myBigInteger: [|BIGINTEGER|];
        myBigText: [|BIGTEXT|];
        myBoolean: [|BOOLEAN|];
        myByte: [|BYTE|];
        myChar: [|CHAR|];
        myClientType: [|CLIENTTYPE|];
        myDataScope: [|DATASCOPE|];
        myDataTransfer: [|DATATRANSFER|];
        myDate: [|DATE|];
        myDateFormula: [|DATEFORMULA|];
        myDateTime: [|DATETIME|];
        myDecimal: [|DECIMAL|];
        myDialog: [|DIALOG|];
        myDuration: [|DURATION|];
        myErrorInfo: [|ERRORINFO|];
        myFieldRef: [|FIELDREF|];
        myFieldType: [|FIELDTYPE|];
        myFile: [|FILE|];
        myFileUpload: [|FILEUPLOAD|];
        myFilterPageBuilder: [|FILTERPAGEBUILDER|];
        myGuid: [|GUID|];
        myHttpClient: [|HTTPCLIENT|];
        myHttpContent: [|HTTPCONTENT|];
        myHttpHeaders: [|HTTPHEADERS|];
        myHttpRequestMessage: [|HTTPREQUESTMESSAGE|];
        myHttpResponseMessage: [|HTTPRESPONSEMESSAGE|];
        myInStream: [|INSTREAM|];
        myInteger: [|INTEGER|];
        myIsolationLevel: [|ISOLATIONLEVEL|];
        myJsonArray: [|JSONARRAY|];
        myJsonObject: [|JSONOBJECT|];
        myJsonToken: [|JSONTOKEN|];
        myJsonValue: [|JSONVALUE|];
        myKeyRef: [|KEYREF|];
        myModuleInfo: [|MODULEINFO|];
        myNotification: [|NOTIFICATION|];
        myNotificationScope: [|NOTIFICATIONSCOPE|];
        myObjectType: [|OBJECTTYPE|];
        myOutStream: [|OUTSTREAM|];
        myPromptMode: [|PROMPTMODE|];
        myRecordId: [|RECORDID|];
        myRecordRef: [|RECORDREF|];
        myReportFormat: [|REPORTFORMAT|];
        myReportLayoutType: [|REPORTLAYOUTTYPE|];
        mySecretText: [|SECRETTEXT|];
        mySecurityFilter: [|SECURITYFILTER|];
        mySessionSettings: [|SESSIONSETTINGS|];
        myTableConnectionType: [|TABLECONNECTIONTYPE|];
        myTelemetryScope: [|TELEMETRYSCOPE|];
        myTestPermissions: [|TESTPERMISSIONS|];
        myTextBuilder: [|TEXTBUILDER|];
        myTextEncoding: [|TEXTENCODING|];
        myTime: [|TIME|];
        myVariant: [|VARIANT|];
        myVersion: [|VERSION|];
        myXmlAttribute: [|XMLATTRIBUTE|];
        myXmlDeclaration: [|XMLDECLARATION|];
        myXmlDocument: [|XMLDOCUMENT|];
        myXmlElement: [|XMLELEMENT|];
        myXmlNamespaceManager: [|XMLNAMESPACEMANAGER|];
        myXmlNode: [|XMLNODE|];
        myXmlNodeList: [|XMLNODELIST|];
        myXmlWriteOptions: [|XMLWRITEOPTIONS|];
}

table 50000 MyTable
{
    fields
    {
        field(1; "Primary Key"; Integer) { }
        field(2; myBigInteger; [|BIGINTEGER|]) { }
        field(3; myBlob; [|BLOB|]) { }
        field(4; myBoolean; [|BOOLEAN|]) { }
        field(5; myDate; [|DATE|]) { }
        field(6; myDateFormula; [|DATEFORMULA|]) { }
        field(7; myDateTime; [|DATETIME|]) { }
        field(8; myDecimal; [|DECIMAL|]) { }
        field(9; myDuration; [|DURATION|]) { }
        field(10; myGuid; [|GUID|]) { }
        field(11; myInteger; [|INTEGER|]) { }
        field(12; myMedia; [|MEDIA|]) { }
        field(13; myMediaSet; [|MEDIASET|]) { }
        field(14; myRecordId; [|RECORDID|]) { }
        field(15; myTableFilter; [|TABLEFILTER|]) { }
        field(16; myTime; [|TIME|]) { }
    }
}