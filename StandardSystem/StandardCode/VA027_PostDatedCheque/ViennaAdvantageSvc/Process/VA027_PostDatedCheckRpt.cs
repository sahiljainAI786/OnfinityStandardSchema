using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace ViennaAdvantage.Process
{
    public class VA027_PostDatedCheckRpt : SvrProcess
    {
        StringBuilder _Sql = new StringBuilder();
        DataSet _ds = new DataSet();

        int _bpartner = 0;
        int _docType = 0;
        private int paymentDocumentTypeId = 0;
        string _showCreate = "";
        DateTime? _sysDate = DateTime.Now;


        protected override string DoIt()
        {
            _Sql.Clear();
            _Sql.Append("DELETE FROM VA027_PDC_Temp");
            int _no = DB.ExecuteQuery(_Sql.ToString(), null, Get_TrxName());
            _Sql.Clear();

            if (_showCreate == "S")//display data
            {
                Show();
                if (_docType > 0)
                {
                    _Sql.Append(" AND C_doctype_id=" + _docType);
                }
                if (_bpartner > 0)
                {
                    if (_docType != 0)
                    {
                        _Sql.Append(" AND C_BPartner_ID=" + _bpartner);
                    }
                    else
                    {
                        _Sql.Append(" AND C_BPartner_ID=" + _bpartner);
                    }
                }
                DB.ExecuteQuery(_Sql.ToString(), null, Get_TrxName());
                return "";
            }

            else if (_showCreate == "C")//display and create data
            {
                // when PDC doc type / payment doc type not selected,not to create or open report
                if (_docType <= 0)
                {
                    return Msg.GetMsg(GetCtx(), "VA027_SelectPDCDocType");
                }
                if (paymentDocumentTypeId <= 0)
                {
                    return Msg.GetMsg(GetCtx(), "VA027_SelectPaymentDocType");
                }

                // create called first so that we can get detail of record whose payment is generated.
                Create();
                
                Show();

                if (_docType > 0)
                {
                    _Sql.Append(" AND C_doctype_id=" + _docType);
                }
                if (_bpartner > 0)
                {
                    if (_docType != 0)
                    {
                        _Sql.Append(" AND C_BPartner_ID=" + _bpartner);
                    }
                    else
                    {
                        _Sql.Append(" AND C_BPartner_ID=" + _bpartner);
                    }
                }
                DB.ExecuteQuery(_Sql.ToString(), null, Get_TrxName());
                 
            }

            return "";
        }

        /// <summary>
        /// Show Data
        /// </summary>
        /// <returns></returns>
        public string Show()
        {
            _Sql.Append(@"INSERT INTO VA027_PDC_Temp(
                                      AD_CLIENT_ID,
                                      AD_ORG_ID,
                                      DOCUMENTNO,
                                      C_BPARTNER_ID,
                                      C_BPARTNER_ID_1,
                                      VA027_CHECKDATE,
                                      VA027_CHECKNO,
                                      VA027_PAYAMT,
                                      VA027_ACCOUNTNO,
                                      VA027_PAYMENTSTATUS,
                                      C_DOCTYPE_ID,
                                      DOCSTATUS,
                                      VA027_Payee,
                                      VA027_SHOWCREATE,
                                      C_DocTypeTarget_ID)");
            //_Sql.Append(@"SELECT AD_CLIENT_ID, AD_ORG_ID, DOCUMENTNO, C_BPARTNER_ID, VA027_PAYAMT, VA027_CHECKNO, VA027_CHECKDATE, VA027_ACCOUNTNO, VA027_PAYMENTSTATUS, C_DOCTYPE_ID, DOCSTATUS,'" + _showCreate + "'");
            //_Sql.Append(@" FROM VA027_POSTDATEDCHECK ");
            _Sql.Append(@"SELECT PDC.AD_CLIENT_ID,
                                  PDC.AD_ORG_ID,
                                  PDC.DOCUMENTNO,
                                  (SELECT name FROM c_bpartner WHERE c_bpartner_id=PDC.C_BPARTNER_ID
                                  ) AS C_BPARTNER_ID,
                                  PDC.C_BPARTNER_ID AS C_BPARTNER_ID_1,
                                  PDC.VA027_CHECKDATE ,
                                  PDC.VA027_CHECKNO ,
                                  PDC.VA027_PAYAMT,
                                  PDC.VA027_ACCOUNTNO ,
                                  PDC.VA027_PAYMENTSTATUS,
                                  PDC.C_DOCTYPE_ID,
                                  PDC.DOCSTATUS,
                                  CASE
                                    WHEN PDC.C_BPARTNER_ID IS NULL
                                    THEN PDC.VA027_Payee
                                  END AS VA027_Payee,
                                  '" + _showCreate + "', " + paymentDocumentTypeId + @"
                                FROM VA027_POSTDATEDCHECK PDC
                                WHERE PDC.DOCSTATUS                         IN ('CO','CL')
                                AND PDC.VA027_CHECKDATE <= SysDate 
                                AND PDC.ISACTIVE                             ='Y'
                                AND PDC.VA027_PAYMENTGENERATED               ='N'
                                AND PDC.VA027_MULTICHEQUE                    ='N'
                                UNION ALL
                                SELECT PDC.AD_CLIENT_ID,
                                  PDC.AD_ORG_ID,
                                  PDC.DOCUMENTNO,
                                  CASE
                                    WHEN C_BPARTNER_ID IS NULL
                                    THEN PDC.VA027_DESCRIPTION
                                    ELSE
                                      (SELECT NAME FROM C_BPARTNER WHERE C_BPARTNER_ID=PDC.C_BPARTNER_ID
                                      )
                                  END AS C_BPARTNER_ID,
                                  PDC.C_BPARTNER_ID AS C_BPARTNER_ID_1,
                                  CD.VA027_CHECKDATE ,
                                  CD.VA027_CHECKNO ,
                                  CD.VA027_CHEQUEAMOUNT AS VA027_PAYAMT,
                                  CD.VA027_ACCOUNTNO,
                                  CD.VA027_PAYMENTSTATUS ,
                                  PDC.C_DOCTYPE_ID,
                                  PDC.DOCSTATUS,
                                  CASE
                                    WHEN PDC.C_BPARTNER_ID IS NULL
                                    THEN PDC.VA027_Payee
                                  END AS VA027_Payee,
                                  '" + _showCreate + "', " + paymentDocumentTypeId + @"
                                FROM VA027_POSTDATEDCHECK PDC
                                LEFT JOIN VA027_CHEQUEDETAILS CD
                                ON CD.VA027_POSTDATEDCHECK_ID               = PDC.VA027_POSTDATEDCHECK_ID
                                WHERE Pdc.Docstatus                        IN ('CO','CL')
                                AND PDC.ISACTIVE                            ='Y'
                                AND CD.VA027_CHECKDATE <= SysDate 
                                AND NVL(CD.C_Payment_ID, 0) <=0 
                                AND PDC.VA027_MULTICHEQUE                   ='Y'
                                UNION ALL
                                SELECT PDC.AD_CLIENT_ID,
                                  PDC.AD_ORG_ID,
                                  PDC.DOCUMENTNO,
                                  (SELECT name FROM c_bpartner WHERE c_bpartner_id=PDC.C_BPARTNER_ID
                                  ) AS C_BPARTNER_ID,
                                  PDC.C_BPARTNER_ID AS C_BPARTNER_ID_1,
                                  PDC.VA027_CHECKDATE ,
                                  PDC.VA027_CHECKNO ,
                                  PDC.VA027_PAYAMT,
                                  PDC.VA027_ACCOUNTNO ,
                                  PDC.VA027_PAYMENTSTATUS,
                                  PDC.C_DOCTYPE_ID,
                                  PDC.DOCSTATUS,
                                  CASE
                                    WHEN PDC.C_BPARTNER_ID IS NULL
                                    THEN PDC.VA027_Payee
                                  END AS VA027_Payee,
                                  '" + _showCreate + "', " + paymentDocumentTypeId + @"
                                FROM VA027_POSTDATEDCHECK PDC
                                WHERE PDC.DOCSTATUS           IN ('CO','CL')
                                AND pdc.va027_discountingpdc   ='Y'
                                AND PDC.ISACTIVE               ='Y'
                                AND PDC.VA027_PAYMENTGENERATED ='N'
                                AND PDC.VA027_MULTICHEQUE      ='N'
                                UNION ALL
                                SELECT PDC.AD_CLIENT_ID,
                                  PDC.AD_ORG_ID,
                                  PDC.DOCUMENTNO,
                                  CASE
                                    WHEN C_BPARTNER_ID IS NULL
                                    THEN PDC.VA027_DESCRIPTION
                                    ELSE
                                      (SELECT NAME FROM C_BPARTNER WHERE C_BPARTNER_ID=PDC.C_BPARTNER_ID
                                      )
                                  END AS C_BPARTNER_ID,
                                  PDC.C_BPARTNER_ID AS C_BPARTNER_ID_1,
                                  CD.VA027_CHECKDATE ,
                                  CD.VA027_CHECKNO ,
                                  CD.VA027_CHEQUEAMOUNT AS VA027_PAYAMT,
                                  CD.VA027_ACCOUNTNO,
                                  CD.VA027_PAYMENTSTATUS ,
                                  PDC.C_DOCTYPE_ID,
                                  PDC.DOCSTATUS,
                                  CASE
                                    WHEN PDC.C_BPARTNER_ID IS NULL
                                    THEN PDC.VA027_Payee
                                  END AS VA027_Payee,
                                  '" + _showCreate + "', " + paymentDocumentTypeId + @"
                                FROM VA027_POSTDATEDCHECK PDC
                                LEFT JOIN VA027_CHEQUEDETAILS CD
                                ON CD.VA027_POSTDATEDCHECK_ID  = PDC.VA027_POSTDATEDCHECK_ID
                                WHERE Pdc.Docstatus           IN ('CO','CL')
                                AND PDC.ISACTIVE               ='Y'
                                AND cd.va027_discountingpdc    ='Y'
                                AND  NVL(CD.C_Payment_ID, 0) <=0
                                AND PDC.VA027_MULTICHEQUE      ='Y'");

            return "";
        }

        /// <summary>
        /// Create Payment
        /// </summary>
        /// <returns></returns>
        public string Create()
        {
            StringBuilder sbRet = new StringBuilder();
            string _systemDate = _sysDate.Value.ToShortDateString();
            _Sql.Clear();
            DataSet _date = new DataSet();
            _Sql.Append(@"SELECT PDC.VA027_CheckDate, PDC.VA027_POSTDATEDCHECK_ID,PDC.DOCUMENTNO FROM VA027_PostDatedCheck PDC WHERE PDC.IsActive = 'Y' AND PDC.VA027_PAYMENTGENERATED='N' AND PDC.VA027_MULTICHEQUE='N' AND PDC.DOCSTATUS IN('CO','CL') AND PDC.AD_Client_ID = " + GetCtx().GetAD_Client_ID());
            if (GetAD_Org_ID() > 0)
            {
                _Sql.Append(" AND PDC.AD_Org_ID = " + GetAD_Org_ID());
            }
           
            _date = DB.ExecuteDataset(_Sql.ToString(), null, Get_TrxName());
            for (int i = 0; i < _date.Tables[0].Rows.Count; i++)
            {
                if (_date.Tables[0].Rows[i]["VA027_CheckDate"].ToString() != string.Empty)
                {
                    DateTime _checkdt = Convert.ToDateTime(_date.Tables[0].Rows[i]["VA027_CheckDate"]);
                    int record_ID = Util.GetValueOfInt(_date.Tables[0].Rows[i]["VA027_PostDatedCheck_ID"]);
                    string _checkDate = _checkdt.ToShortDateString();
                    if (Convert.ToDateTime(_checkDate) <= Convert.ToDateTime(_systemDate)) //changes made by arpit
                    {
                        ViennaAdvantage.Process.VA027_GenPayment _genPayment = new ViennaAdvantage.Process.VA027_GenPayment();
                        string result = _genPayment.GenratePaymentHdr(GetCtx(), record_ID, paymentDocumentTypeId, Get_TrxName());
                        if (result == "E")
                        {
                            if (sbRet.Length != 0)
                            {
                                sbRet.Append(", " + Util.GetValueOfInt(_date.Tables[0].Rows[i]["DocumentNo"]));
                            }
                            else
                            {
                                sbRet.Append(Msg.GetMsg(GetCtx(), "VA027_PaymentsNotSaved") + Util.GetValueOfInt(_date.Tables[0].Rows[i]["DocumentNo"]));
                            }
                        }
                    }
                }
            }

            _Sql.Clear();
            _date.Dispose();


            _Sql.Append(@"SELECT PDC.VA027_CheckDate, PDC.VA027_POSTDATEDCHECK_ID,PDC.DOCUMENTNO FROM VA027_PostDatedCheck PDC WHERE PDC.IsActive = 'Y' AND PDC.VA027_PAYMENTGENERATED='N' AND PDC.DOCSTATUS IN('CO','CL') AND PDC.VA027_MULTICHEQUE='Y' AND PDC.AD_Client_ID = " + GetCtx().GetAD_Client_ID());
            if (GetAD_Org_ID() > 0)
            {
                _Sql.Append(" AND PDC.AD_Org_ID = " + GetAD_Org_ID());
            }
            _date = DB.ExecuteDataset(_Sql.ToString(), null, Get_TrxName());
            for (int i = 0; i < _date.Tables[0].Rows.Count; i++)
            {
                int record_ID = Util.GetValueOfInt(_date.Tables[0].Rows[i]["VA027_PostDatedCheck_ID"]);
                ViennaAdvantage.Process.VA027_GenPayment _genPayment = new ViennaAdvantage.Process.VA027_GenPayment();
                string result = _genPayment.GenratePaymentLine(GetCtx(), record_ID, paymentDocumentTypeId, Get_TrxName());
                if (result == "E")
                {
                    if (sbRet.Length != 0)
                    {
                        sbRet.Append(", " + Util.GetValueOfInt(_date.Tables[0].Rows[i]["DocumentNo"]));
                    }
                    else
                    {
                        sbRet.Append(Msg.GetMsg(GetCtx(), "VA027_PaymentsNotSaved") + Util.GetValueOfInt(_date.Tables[0].Rows[i]["DocumentNo"]));
                    }
                }
            }
            if (sbRet.Length != 0)
            {
                sbRet.Append(Msg.GetMsg(GetCtx(), "VA027_PaymentGenerated"));
            }
            _Sql.Clear();
            return sbRet.ToString();
        }

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VA027_ShowCreate"))
                {
                    _showCreate = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (name.Equals("C_BPartner_ID_1"))
                {
                    _bpartner = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_DocType_ID"))
                {
                    _docType = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_DocTypeTarget_ID"))
                {
                    paymentDocumentTypeId = Util.GetValueOfInt(para[i].GetParameter());
                }
            }
        }
    }
}
