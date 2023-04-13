using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThomsonReuters.BackgroundOperations.Messaging;

namespace SampleClientApp.Common
{
    public class BackgroundOperationsFactory
    {
        public const string MESSAGE_SOURCE_FUSION = "Fusion";

        public const string MESSAGE_TYPE_DOCUMENT_ANALYSIS = "DocumentAnalysis";
        public const string MESSAGE_TYPE_MASSCREATEACTIONS = "MassCreateActions";
        public const string MESSAGE_TYPE_MASSNOTIFYACTIONS = "MassNotifyActions";
        public const string MESSAGE_TYPE_REPORTS = "Reports";
        public const string MESSAGE_TYPE_DELETE_TEMP_REPORTS = "DeleteTempReports";
        public const string MESSAGE_TYPE_TICKETBAICANCEL = "TicketBaiCancel";
        public const string MESSAGE_TYPE_TICKETBAIPUBLISHING = "TicketBaiPublishing";
        public const string MESSAGE_TYPE_LAWFIRMVALIDATE = "LawFirmValidate";
        public const string MESSAGE_TYPE_LFFIXINCONSISTENCIES = "LfFixInconsistencies";
        public const string MESSAGE_TYPE_DELETELEFTOVERDOCUMENTS = "DeleteLeftoverDocuments";

        static BackgroundOperationsFactory()
        {
            MessageFactory = new MessageFactory();
        }

        public static MessageFactory MessageFactory { get; private set; }
    }


    public class AuthData
    {
        public int UserDataID { get; set; }

        public int LawFirmID { get; set; }

        public string Account { get; set; }

        public int JobID { get; set; }

        public string App { get; set; }

        public int Owner { get; set; }

        public string Tenant { get; set; }

    }


}