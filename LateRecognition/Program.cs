using System;
using System.Collections.Generic;
using SAPApi;
using SAPbobsCOM;
using SAPbouiCOM.Framework;

namespace LateRecognition
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application oApp = null;
                oApp = args.Length < 1 ? new Application() : new Application(args[0]);
                XCompany = (Company)Application.SBO_Application.Company.GetDICompany();
                Menu MyMenu = new Menu();
                Recordset recSet1 = (Recordset)XCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = "SELECT LinkAct_18  FROM OACP where PeriodCat ='" +
                               DateTime.Now.Year + "'";
                recSet1.DoQuery(DIManager.QueryHanaTransalte(query));

                DownPaymentTaxOffsetAcct = recSet1.Fields.Item("LinkAct_18").Value.ToString();
                MyMenu.AddMenuItems();
                oApp.RegisterMenuEventHandler(MyMenu.SBO_Application_MenuEvent);
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                oApp.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public static string DownPaymentTaxOffsetAcct { get; private set; }
        public static SAPbobsCOM.Company XCompany;
        static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    //Exit Add-On
                    System.Windows.Forms.Application.Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    break;
                default:
                    break;
            }
        }
    }
}
