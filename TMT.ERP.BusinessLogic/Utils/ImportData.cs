using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;
using CommonLib;
using System.Globalization;

namespace TMT.ERP.BusinessLogic.Utils
{
    public class ImportData
    {
        public static void ImportDataFromCSV(string csvFile, string delimiter, string logFolder, int id)
        {
            if (System.IO.File.Exists(csvFile))
            {
                int number = 0;
                int totalSuccessfull = 0;
                int totalFail = 0;

                string logFile = logFolder + "\\Import_" + DateTime.Now.ToString("yyyyMMddhhss") + ".txt";
                System.IO.TextWriter writeFile = File.AppendText(logFile);
                try
                {
                    //Csv parser
                    using (GenericParsing.GenericParser data = new GenericParsing.GenericParser())
                    {
                        data.SetDataSource(csvFile);
                        data.ColumnDelimiter = char.Parse(delimiter);
                        data.FirstRowHasHeader = true;
                        data.MaxBufferSize = 4096000;
                        data.MaxRows = 5000;
                        data.TextQualifier = '\"';
                        DateTime startImport = DateTime.Now;
                        var manager = new GenericManager<BankStatement>();
                        var managerDetail = new GenericManager<BankStatementDetail>();
                        var managerAccTrans = new GenericManager<AccountTran>();

                        //Add to Bank Statement
                        BankStatement objBankStatement = new BankStatement();
                        objBankStatement.BankAccountID = id;
                        objBankStatement.ImportedDate = DateTime.Now;
                        objBankStatement.StartDate = DateTime.Now;
                        objBankStatement.EndDate = DateTime.Now;
                        objBankStatement.Status = 0;
                        manager.Add(objBankStatement);
                        manager.Save();

                        while (data.Read())
                        {
                            try
                            {
                                //Import 
                                var date = data["Date"];
                                var type = data["Type"];
                                var payee = data["Payee"];
                                var particulars = data["Particulars"];
                                var code = data["Code"];
                                var reference = data["Reference"];
                                var analysiscode = data["AnalysisCode"];
                                var spent = data["Spent"];
                                var received = data["Received"];

                                //Add to Bank Statement Detail
                                BankStatementDetail objBankStmDetail = new BankStatementDetail();
                                objBankStmDetail.BankStatementID = objBankStatement.ID;
                                objBankStmDetail.Date = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                                objBankStmDetail.Type = 0;
                                objBankStmDetail.Payee = payee;
                                objBankStmDetail.Particulars = particulars;
                                objBankStmDetail.Code = code;
                                objBankStmDetail.Reference = reference;
                                objBankStmDetail.AnalysisCode = analysiscode;
                                objBankStmDetail.Spent = Double.Parse(spent);
                                objBankStmDetail.Received = Double.Parse(received);
                                objBankStmDetail.Status = 0;
                                managerDetail.Add(objBankStmDetail);
                                managerDetail.Save();

                                //Add to Account Trans
                                AccountTran objAccount = new AccountTran();
                                objAccount.BankAccountID = id;
                                objAccount.Spent = Double.Parse(spent);
                                objAccount.Received = Double.Parse(received);
                                objAccount.Date = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                                objAccount.Status = 0;
                                managerAccTrans.Add(objAccount);
                                managerAccTrans.Save();

                                totalSuccessfull++;
                                number++;
                                writeFile.WriteLine("Item Number " + number.ToString() + " success");
                            }
                            catch (Exception ex)
                            {
                                totalFail++;
                                writeFile.WriteLine("Item Number " + number.ToString() + " fail");
                                writeFile.WriteLine("Exception : " + ex.ToString());
                            }

                        }
                        
                        writeFile.WriteLine("-----------------------------------------------------");
                        writeFile.WriteLine("Start imported time: " + startImport.ToShortDateString());
                        writeFile.WriteLine("finish time: " + DateTime.Now);
                        writeFile.WriteLine("Total Bank Statements: " + number);
                        writeFile.WriteLine("Number successfull: " + totalSuccessfull.ToString());
                        writeFile.WriteLine("Number fail: " + totalFail.ToString());

                        writeFile.Flush();
                        writeFile.Close();
                        writeFile = null;

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally
                {
                }

            }
        }

    }
}
