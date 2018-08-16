using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using Application = SAPbouiCOM.Framework.Application;

namespace LateRecognition
{
    [FormAttribute("10000008", "BankStatementRowDetailsExplanded.b1f")]
    class BankStatementRowDetailsExplanded : SystemFormBase
    {
        public BankStatementRowDetailsExplanded()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("Item_0").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_2").Specific));
            this.Button0.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.Button0_PressedAfter);
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("Item_3").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_4").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("Item_5").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_6").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {

        }

        private void OnPressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (Application.SBO_Application.Forms.ActiveForm.Type != 392)
            {
                return;
            }

            AddJournalEntryBankExpanded(EditText1.Value);
        }

        private void AddJournalEntryBankExpanded(string amount)
        {
            if (string.IsNullOrWhiteSpace(amount))
            {
                return;
            }

            if (decimal.Parse(amount) >= 0)
            {

                ((ComboBox)Application.SBO_Application.Forms.ActiveForm.Items.Item("1320002034").Specific).Select(BankStatementDetailsModel.Branch); //Branch;
                ((ComboBox)Application.SBO_Application.Forms.ActiveForm.Items.Item("9").Specific).Select(2, BoSearchKey.psk_Index); //Trans Code;
                ((EditText)Application.SBO_Application.Forms.ActiveForm.Items.Item("6").Specific).Value = BankStatementDetailsModel.StatemenRowDate.ToString("yyyyMMdd"); //Posting Date;
                ((EditText)Application.SBO_Application.Forms.ActiveForm.Items.Item("102").Specific).Value = BankStatementDetailsModel.StatemenDueDate.ToString("yyyyMMdd"); //Posting Date;
                ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                    .Item("6").Cells.Item(1).Specific).Value = (decimal.Parse(amount) * -1).ToString();//Credit
                ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                    .Item("1").Cells.Item(2).Specific).Value = Program.DownPaymentTaxOffsetAcct;//account
                ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                    .Item("6").Cells.Item(2).Specific).Value = decimal.Parse(amount).ToString();//Debit
            }
            else
            {
                ((ComboBox)Application.SBO_Application.Forms.ActiveForm.Items.Item("1320002034").Specific).Select(BankStatementDetailsModel.Branch); //Branch;
                ((ComboBox)Application.SBO_Application.Forms.ActiveForm.Items.Item("9").Specific).Select(2, BoSearchKey.psk_Index); //Trans Code;
                ((EditText)Application.SBO_Application.Forms.ActiveForm.Items.Item("6").Specific).Value = BankStatementDetailsModel.StatemenRowDate.ToString("yyyyMMdd"); //Posting Date;
                ((EditText)Application.SBO_Application.Forms.ActiveForm.Items.Item("102").Specific).Value = BankStatementDetailsModel.StatemenDueDate.ToString("yyyyMMdd"); //Posting Date;
                ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                    .Item("6").Cells.Item(1).Specific).Value = decimal.Parse(amount).ToString();//Debit
                ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                    .Item("1").Cells.Item(2).Specific).Value = Program.DownPaymentTaxOffsetAcct;//account
                ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                    .Item("6").Cells.Item(2).Specific).Value = (decimal.Parse(amount) * -1).ToString();//Credit


            }
        }

        private SAPbouiCOM.EditText EditText0;

        private void OnCustomInitialize()
        {
            ((Button)GetItem("350000046").Specific).PressedAfter += OnPressedAfter; ;
        }

        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.Button Button0;

        private void Button0_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            Matrix matrix = (Matrix)GetItem("10000012").Specific;
            if (matrix.RowCount == 0)
            {
                return;
            }

            BankStatementDetailsModel.BpCode = ((EditText)GetItem("360000038").Specific).Value;

            int chechedCount = 0;

            for (int i = 0; i < matrix.RowCount; i++)
            {
                var @checked = ((CheckBox)matrix.Columns.Item("10000021").Cells.Item(i + 1).Specific).Checked;
                if (@checked)
                {
                    chechedCount++;
                    string originalAmountFcString = ((EditText)matrix.Columns.Item("360000041").Cells.Item(i + 1).Specific).Value;
                    string balanceDueString = ((EditText)matrix.Columns.Item("10000013").Cells.Item(i + 1).Specific).Value;
                    decimal balanceDue = decimal.Parse(balanceDueString.Split(' ')[0]);
                    string document = ((EditText)matrix.Columns.Item("10000035").Cells.Item(i + 1).Specific).Value;
                    int indexOfhyphen = document.IndexOf("-");
                    int indexOfSlash = document.IndexOf("/");
                    Recordset recSet =
                        (Recordset)Program.XCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    string docType = document.Substring(0, 2);
                    int docNum = int.Parse(document.Substring(indexOfhyphen + 1, indexOfSlash - indexOfhyphen - 1));
                    recSet.DoQuery($"select  BplName from ODPI where docNum = {docNum}");
                    BankStatementDetailsModel.Branch = recSet.Fields.Item("BplName").Value.ToString();

                    string originalAmountFc = originalAmountFcString.Split(' ')[0];
                    string currency = originalAmountFcString.Split(' ')[originalAmountFcString.Split(' ').Length - 1];

                    string appliedAmountLcstringLc = ((EditText)matrix.Columns.Item("10000017").Cells.Item(i + 1).Specific).Value;
                    string appliedAmountLc = appliedAmountLcstringLc.Split(' ')[0];

                    BankStatementDetailsModel.OriginalAmountFc = decimal.Parse(originalAmountFc.Replace("(", "-").Replace(")", ""));
                    BankStatementDetailsModel.Currency = currency;
                    BankStatementDetailsModel.AppliedAmountFc = decimal.Parse(appliedAmountLc);
                    decimal diff = 0;
                    decimal realAmount = BankStatementDetailsModel.CalculateAppliedAmount(ref diff);
                    EditText0.Value = realAmount.ToString();
                    EditText1.Value = diff.ToString();
                    if (realAmount - balanceDue > 0)
                    {
                        Application.SBO_Application.MessageBox($"საჯიროა ახალი დოკუმენტის შექმნა თანხით {realAmount - balanceDue}");
                        ((EditText)matrix.Columns.Item("10000017").Cells.Item(i + 1).Specific).Value = (balanceDue).ToString();
                        EditText2.Value  = (realAmount - balanceDue).ToString();
                        break;
                    }

                        ((EditText)matrix.Columns.Item("10000017").Cells.Item(i + 1).Specific).Value = realAmount.ToString();
                    break;


                }
            }

            if (chechedCount == 0)
            {
                return;
            }



        }

        private EditText EditText1;
        private StaticText StaticText1;
        private EditText EditText2;
        private StaticText StaticText2;
    }
}
