using System;

using System.Globalization;

using SAPbouiCOM;
using SAPbouiCOM.Framework;
using Application = SAPbouiCOM.Framework.Application;

namespace LateRecognition
{
    [FormAttribute("10000005", "Forms/BankStatementDetails.b1f")]
    class BankStatementDetails : SystemFormBase
    {
        public BankStatementDetails()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Matrix0 = ((SAPbouiCOM.Matrix)(this.GetItem("10000036").Specific));
            this.Matrix0.DoubleClickBefore += new SAPbouiCOM._IMatrixEvents_DoubleClickBeforeEventHandler(this.Matrix0_DoubleClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private SAPbouiCOM.Matrix Matrix0;

        private void Matrix0_DoubleClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.ColUID != "10000000")
            {
                return;
            }

            Matrix matrix = (Matrix)(GetItem("10000036").Specific);
            DateTime rowDate = DateTime.ParseExact(((EditText)matrix.Columns.Item("10000033").Cells.Item(pVal.Row).Specific).Value, "yyyyMMdd", CultureInfo.InvariantCulture);//Posting date
            DateTime dueDate = DateTime.ParseExact(((EditText)matrix.Columns.Item("10000031").Cells.Item(pVal.Row).Specific).Value, "yyyyMMdd", CultureInfo.InvariantCulture);//Due date 
            string incomingAmountPaymentCurrencyString = ((EditText)matrix.Columns.Item("10000043").Cells.Item(pVal.Row).Specific).Value;

            decimal incomingPaymentAmountLc = string.IsNullOrWhiteSpace(incomingAmountPaymentCurrencyString) ? 0 : decimal.Parse(incomingAmountPaymentCurrencyString.Split(' ')[0]);//Incoming Amt - Payment Currency

            string branch = ((EditText)matrix.Columns.Item("234000093").Cells.Item(pVal.Row).Specific).Value;//bRanch

            string bpCode = ((EditText)matrix.Columns.Item("10000011").Cells.Item(pVal.Row).Specific).Value;//Business Partner Code


            BankStatementDetailsModel.BpCode = bpCode;
            BankStatementDetailsModel.Branch = branch;
            BankStatementDetailsModel.IncomingAmountLc = incomingPaymentAmountLc;
            BankStatementDetailsModel.StatemenDueDate = dueDate;
            BankStatementDetailsModel.StatemenRowDate = rowDate;
            Matrix0.GetLineData(pVal.Row);

        }

        private void OnCustomInitialize()
        {

        }
    }
}
