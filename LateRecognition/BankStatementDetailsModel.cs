using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace LateRecognition
{
    public static class BankStatementDetailsModel
    {
        public static DateTime StatemenRowDate { get; set; }
        public static DateTime StatemenDueDate { get; set; }
        public static string Currency { get; set; }
        public static decimal IncomingAmountLc { get; set; }
        public static decimal OriginalAmountFc { get; set; }
        public static decimal AppliedAmountFc { get; set; }
         
        public static string BpCode { get; set; }
        public static string Branch { get; set; }
        public static decimal CalculateAppliedAmount(ref decimal Diff)
        {
            decimal exchangeRateOld = (decimal)SAPApi.UiManager.GetCurrencyRate(Currency, StatemenDueDate, Program.XCompany);
            decimal exchangeRateNow = (decimal)SAPApi.UiManager.GetCurrencyRate(Currency, StatemenRowDate, Program.XCompany);
            decimal realAmount = IncomingAmountLc / exchangeRateOld;
   
            Diff = Math.Round(IncomingAmountLc - Math.Round(realAmount,2) * exchangeRateNow,2);
         
            return Math.Round(realAmount,2);
        }
    } 
  
    
}
