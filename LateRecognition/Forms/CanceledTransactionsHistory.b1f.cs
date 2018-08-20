 
using SAPbouiCOM;
using SAPbouiCOM.Framework;

namespace LateRecognition.Forms
{
    [FormAttribute("LateRecognition.Forms.CanceledTransactionsHistory", "Forms/CanceledTransactionsHistory.b1f")]
    class CanceledTransactionsHistory : UserFormBase
    {
        public   bool _isFormOpen;


        public CanceledTransactionsHistory()
        {
        }
        public CanceledTransactionsHistory(bool isFormOpen)
        {
            _isFormOpen = isFormOpen;
        }
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("Item_0").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new CloseAfterHandler(this.Form_CloseAfter);

        }

        private SAPbouiCOM.Grid Grid0;

        private void OnCustomInitialize()
        {
            RefreshHistory();
        }

        public void RefreshHistory()
        {
            Grid0.DataTable.ExecuteQuery(@"Select   x1.number as [Canceled],   x2.Number  as [Cancelation] FROM OJDT x1  inner join 
               OJDT x2  on x1.number  = x2.ExTransId where  x2.ExTransId is not null");
            SAPbouiCOM.EditTextColumn oColumns = (EditTextColumn)Grid0.Columns.Item("Canceled");
            oColumns.LinkedObjectType = "30";
            SAPbouiCOM.EditTextColumn oColumns1 = (EditTextColumn)Grid0.Columns.Item("Cancelation");
            oColumns1.LinkedObjectType = "30";
        }

        private void Form_CloseAfter(SBOItemEventArg pVal)
        {
            _isFormOpen = false;
        }
    }
}
