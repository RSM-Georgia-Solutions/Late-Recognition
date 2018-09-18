
using System;
using SAPbobsCOM;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using Application = SAPbouiCOM.Framework.Application;

namespace LateRecognition
{
    [FormAttribute("10000008", "Forms/BankStatementRowDetailsExplanded.b1f")]
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
            this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);

        }

        private void OnPressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (Application.SBO_Application.Forms.ActiveForm.Type != 392)
            {
                return;
            }

            AddJournalEntryBankExpanded(EditText1.Value);
        }
        /// <summary>
        /// Add Journal Antry for Baks tatement
        /// </summary>
        /// <param name="amount"></param>
        private void AddJournalEntryBankExpanded(string amount)
        {
            if (string.IsNullOrWhiteSpace(amount))
            {
                return;
            }


                ((ComboBox)Application.SBO_Application.Forms.ActiveForm.Items.Item("1320002034").Specific).Select(BankStatementDetailsModel.Branch); //Branch;
            ((ComboBox)Application.SBO_Application.Forms.ActiveForm.Items.Item("9").Specific).Select("2"); //Trans Code;
            ((EditText)Application.SBO_Application.Forms.ActiveForm.Items.Item("6").Specific).Value = BankStatementDetailsModel.StatemenRowDate.ToString("yyyyMMdd"); //Posting Date;
            ((EditText)Application.SBO_Application.Forms.ActiveForm.Items.Item("7").Specific).Value =
                BankStatementDetailsModel.DownPaymentDocNum;//Dpm Doc Num
            ((EditText)Application.SBO_Application.Forms.ActiveForm.Items.Item("102").Specific).Value = BankStatementDetailsModel.StatemenDueDate.ToString("yyyyMMdd"); //DueDate;
            ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                .Item("6").Cells.Item(1).Specific).Value = (decimal.Parse(amount) * -1).ToString();//Credit
            ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                .Item("1").Cells.Item(2).Specific).Value = Program.DownPaymentTaxOffsetAcct;//G/L Acct/BP Code
            ((EditText)((Matrix)Application.SBO_Application.Forms.ActiveForm.Items.Item("76").Specific).Columns
                .Item("6").Cells.Item(2).Specific).Value = decimal.Parse(amount).ToString();//Credit



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
            int chechedCountx = 0;
            for (int i = 0; i < matrix.RowCount; i++)
            {
                var @checked = ((CheckBox)matrix.Columns.Item("10000021").Cells.Item(i + 1).Specific).Checked;
                if (@checked)
                {
                    chechedCountx++;
                }
            }
            if (chechedCountx != 1)
            {
                return;
            }
            for (int i = 0; i < matrix.RowCount; i++)
            {
                var @checked = ((CheckBox)matrix.Columns.Item("10000021").Cells.Item(i + 1).Specific).Checked;
                if (@checked)
                {
                    chechedCount++;
                    string originalAmountFcString = ((EditText)matrix.Columns.Item("360000041").Cells.Item(i + 1).Specific).Value;
                    string balanceDueString = ((EditText)matrix.Columns.Item("10000013").Cells.Item(i + 1).Specific).Value;
                    decimal balanceDue = 0;
                    try
                    {
                          balanceDue = decimal.Parse(balanceDueString.Split(' ')[0]); 
                    }
                    catch (Exception)
                    { 
                       return;
                    }


                    string document = ((EditText)matrix.Columns.Item("10000035").Cells.Item(i + 1).Specific).Value;
                    int indexOfhyphen = document.IndexOf("-");
                    int indexOfSlash = document.IndexOf("/");
                    Recordset recSet =
                        (Recordset)Program.XCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    string docType = document.Substring(0, 2);
                    BankStatementDetailsModel.DocType = docType;
                    int docNum = int.Parse(document.Substring(indexOfhyphen + 1, indexOfSlash - indexOfhyphen - 1));
                    BankStatementDetailsModel.DownPaymentDocNum =  docNum.ToString();
                    switch (BankStatementDetailsModel.DocType)
                    {
                        case "IN":
                            recSet.DoQuery($"select  BplName, DocTotalFC, DocCur  from OINV where docNum = {docNum}");
                            break;
                        case "DT":
                            recSet.DoQuery($"select  BplName, DocTotalFC, DocCur  from ODPI where docNum = {docNum}");
                            break;
                        case "JE":
                            recSet.DoQuery($"select BplName, SUM(FCCredit) DocTotalFC,  ISNULL( FCCurrency, 'GEL') as DocCur, * from jdt1 where TransId = {docNum} and ShortName = '{BankStatementDetailsModel.BpCode}'");
                            break;
                        default: return;
                    }


                    BankStatementDetailsModel.Branch = recSet.Fields.Item("BplName").Value.ToString();
                    BankStatementDetailsModel.OriginalAmountFc = decimal.Parse(recSet.Fields.Item("DocTotalFC").Value.ToString());
                    BankStatementDetailsModel.Currency = recSet.Fields.Item("DocCur").Value.ToString();


                    string appliedAmountLcstringLc = ((EditText)matrix.Columns.Item("10000017").Cells.Item(i + 1).Specific).Value;
                    string appliedAmountLc = appliedAmountLcstringLc.Split(' ')[0];//Applied Amt - Payment Currency



                    BankStatementDetailsModel.AppliedAmountFc = decimal.Parse(appliedAmountLc);
                    decimal diff = 0;
                    decimal realAmount = BankStatementDetailsModel.CalculateAppliedAmount(ref diff);
                    EditText0.Value = realAmount.ToString();
                    EditText1.Value = diff.ToString();
                    if (realAmount - balanceDue > 0)
                    {
                        Application.SBO_Application.MessageBox($"საჭიროა ახალი ავანსის  დოკუმენტის შექმნა");
                        ((EditText)matrix.Columns.Item("10000017").Cells.Item(i + 1).Specific).Value = (balanceDue).ToString();
                        EditText2.Value = (realAmount - balanceDue).ToString();
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

        private void Form_ResizeAfter(SBOItemEventArg pVal)
        {
            GetItem("Item_2").Top = GetItem("10000001").Top;
            GetItem("Item_2").Left = GetItem("10000001").Left + 240;
            GetItem("Item_2").RightJustified = GetItem("10000001").RightJustified;
            GetItem("Item_2").Height = GetItem("10000001").Height;
            GetItem("Item_2").Width = GetItem("10000001").Width;
        }
    }
}
