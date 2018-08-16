using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using SAPbouiCOM;
using SAPbouiCOM.Framework;

namespace LateRecognition
{
    [FormAttribute("LateRecognition.CancelJournalEntrys", "Forms/CancelJournalEntrys.b1f")]
    class CancelJournalEntrys : UserFormBase
    {
        public CancelJournalEntrys()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("Item_0").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_1").Specific));
            this.Button0.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.Button0_PressedAfter);
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("Item_2").Specific));
            this.Button1.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.Button1_PressedAfter);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private SAPbouiCOM.Grid Grid0;

        private void OnCustomInitialize()
        {
            Grid0.DataTable.ExecuteQuery("Select 'N' as [Select], * FROM OJDT WHERE TransCode = '3' ");
            Grid0.Columns.Item("Select").Type = BoGridColumnType.gct_CheckBox;
            SAPbouiCOM.EditTextColumn oColumns = (EditTextColumn)Grid0.Columns.Item("TransId");
            oColumns.LinkedObjectType = "30";
        }

        private Button Button0;

        private void Button0_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            for (int i = 0; i < Grid0.Rows.Count; i++)
            {
                if (Grid0.DataTable.Columns.Item("Select").Cells.Item(i).Value.ToString() == "Y")
                {
                    JournalEntries journalEntryOld = (SAPbobsCOM.JournalEntries)Program.XCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                    journalEntryOld.GetByKey(int.Parse(Grid0.DataTable.Columns.Item("TransId").Cells.Item(i).Value
                        .ToString()));
                    ///////////////////////////////////
                    JournalEntries journalEntryReverce = (SAPbobsCOM.JournalEntries)Program.XCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);

                    journalEntryReverce.DueDate = journalEntryOld.DueDate;
                    journalEntryReverce.ReferenceDate = journalEntryOld.ReferenceDate;
                    journalEntryReverce.Reference = journalEntryOld.Reference;
                    journalEntryReverce.Memo = journalEntryOld.Number.ToString();

                    for (int j = 0; j < journalEntryOld.Lines.Count; j++)
                    {
                        journalEntryOld.Lines.SetCurrentLine(j);

                        journalEntryReverce.Lines.AccountCode = journalEntryOld.Lines.AccountCode;
                        journalEntryReverce.Lines.ShortName = journalEntryOld.Lines.ShortName;
                        journalEntryReverce.Lines.ControlAccount = journalEntryOld.Lines.ControlAccount;
                        journalEntryReverce.Lines.Debit = -journalEntryOld.Lines.Debit;
                        journalEntryReverce.Lines.Credit = -journalEntryOld.Lines.Credit;
                        journalEntryReverce.Lines.ContraAccount = journalEntryOld.Lines.ContraAccount;
                        journalEntryReverce.Lines.BPLID = journalEntryOld.Lines.BPLID; 
                        journalEntryReverce.Lines.Add();
                  
                    }
                    var x =  journalEntryReverce.Add();
                    var x1 = Program.XCompany.GetLastErrorDescription();
                }
                
            }
          
        }

        private Button Button1;

        private void Button1_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            JournalEntries journalEntryOld =
                (SAPbobsCOM.JournalEntries) Program.XCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes
                    .oJournalEntries);
            journalEntryOld.GetByKey(2079);

            for (int i = 0; i < journalEntryOld.Lines.Count; i++)
            {
                journalEntryOld.Lines.SetCurrentLine(i);
            }
        }
    }
}
